<!--
 * @Author: lidonghui
 * @Date: 2024-08-08 15:09:41
 * @LastEditTime: 2024-08-08 15:09:41  
 * @FilePath: \components\dashboard\LiveChart.vue
-->
<template>
    <el-row :gutter="20">
        <el-col :lg="6" :md="12" :sm="12" :xs="24">
            <div class="ve-card ve_card1" @click="handleTodo()">
                <el-icon>
                    <bell />
                </el-icon>
                <div>
                    <p>我的待办</p>
                    <span>{{ todoFrom.todoCount }}</span>
                </div>
            </div>
        </el-col>
        <el-col :lg="6" :md="12" :sm="12" :xs="24">
            <div class="ve-card ve_card2" @click="handleTodayDone()">
                <el-icon>
                    <EditPen />
                </el-icon>
                <div>
                    <p>今日已办</p>
                    <span>{{ todoFrom.doneTodayCount }}</span>
                </div>
            </div>
        </el-col>
        <el-col :lg="6" :md="12" :sm="12" :xs="24">
            <div class="ve-card ve_card3" @click="handleTodayCreate()">
                <el-icon>
                    <TakeawayBox />
                </el-icon>
                <div>
                    <p>今日发起</p>
                    <span>{{ todoFrom.doneCreateCount }}</span>
                </div>
            </div>
        </el-col>
        <el-col :lg="6" :md="12" :sm="12" :xs="24">
            <div class="ve-card ve_card4" @click="handleTodayDraft()">
                <el-icon>
                    <document />
                </el-icon>
                <div>
                    <p>我的草稿</p>
                    <span><el-icon size="50">
                            <Right />
                        </el-icon>
                    </span>
                </div>
            </div>
        </el-col>
    </el-row>
</template>

<script setup name="Index">
import { getTodoList } from "@/api/workflow/index.js";
const { proxy } = getCurrentInstance();

let todoFrom = ref({
    todoCount: 0,
    doneTodayCount: 0,
    doneCreateCount: 0,
})
const getTodo = () => {
    getTodoList().then(res => {
        console.log("获取todolist成功=" ,res.data);
        todoFrom.value = res.data;
    }).catch((err) => {
        if (err && err.msg)
            console.log("获取todolist失败=" + JSON.stringify(err.msg));
    })
}
getTodo();
const handleTodo = () => {
    const obj = { path: "/flowtask/pendding" };
    proxy.$tab.closeOpenPage(obj);
}
const handleTodayDone = () => {
    const obj = { path: "/flowtask/approved" };
    proxy.$tab.closeOpenPage(obj);
}
const handleTodayCreate = () => {
    const obj = { path: "/flowtask/mytask" };
    proxy.$tab.closeOpenPage(obj);
}
const handleTodayDraft = () => {
    const obj = { path: "/flowtask/copyToMe" };
    proxy.$tab.openPage(obj);
}
</script>
<style lang="scss" scoped>
.el-row {
    padding-right: 10px;
}

.el-col {
    margin-bottom: 10px;
}

.ve-card {
    cursor: pointer;
    border-radius: 10px;
    height: 100%;
    display: flex;
    align-items: center;
    transition: all 500ms;
    color: #fff;

    &:hover {
        box-shadow: 3px 3px 6px 1px rgba(0, 0, 0, 0.2);
        background: #fff;
    }

    i {
        font-size: 90px;
        margin: 10px 20px;
    }

    div {
        flex: 1;
        padding-right: 12px;

        p {
            margin: 0;
            white-space: nowrap;
            overflow: hidden;
            text-overflow: ellipsis;
        }

        span {
            font-size: 60px;
            font-weight: bold;
        }
    }
}

.ve_card1 {
    background: #3370ff;

    &:hover {
        color: #3370ff;
    }
}

.ve_card2 {
    background: #13ce66;

    &:hover {
        color: #13ce66;
    }
}

.ve_card3 {
    background: #fe7300;

    &:hover {
        color: #fe7300;
    }
}

.ve_card4 {
    background: #ff4949;

    &:hover {
        color: #ff4949;
    }
}
</style>
