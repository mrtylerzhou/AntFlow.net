<!--
 * @Date:  2024-05-25 14:05:59
 * @LastEditors: LDH 574427343@qq.com
 * @LastEditTime: 2023-05-24 15:17:13
 * @FilePath: /ant-flow/src/components/drawer/promoterDrawer.vue
-->
<template>
    <el-drawer :append-to-body="true" title="发起人" v-model="visible" class="set_promoter" :with-header="false"
        :size="680">
        <span class="drawer-title">发起人</span>
        <div class="demo-drawer__content">
            <div class="promoter_content drawer_content">
                <p>{{ $func.arrToStr(flowPermission) || '发起人' }}</p>
                <el-button type="primary" @click="addPromoter">添加/修改发起人</el-button>
            </div>
            <div class="demo-drawer__footer clear">
                <el-button type="primary" @click="savePromoter">确 定</el-button>
                <el-button @click="closeDrawer">取 消</el-button>
            </div>

        </div>
    </el-drawer>
</template>
<script setup>
import $func from '@/utils/antflow/index'
import { useStore } from '@/store/modules/outsideflow'
import { computed, ref, watch } from 'vue'
let flowPermission = ref([])
let promoterVisible = ref(false)
let checkedList = ref([])

let store = useStore()
let { setPromoter, setFlowPermission } = store
let promoterDrawer = computed(() => store.promoterDrawer)
let flowPermission1 = computed(() => store.flowPermission1)
let visible = computed({
    get() {
        return promoterDrawer.value
    },
    set() {
        closeDrawer()
    }
})
watch(flowPermission1, (val) => {
    flowPermission.value = val.value
})

const addPromoter = () => {
    checkedList.value = flowPermission.value
    promoterVisible.value = true;
}
const surePromoter = (data) => {
    flowPermission.value = data;
    promoterVisible.value = false;
}
const savePromoter = () => {
    setFlowPermission({
        value: flowPermission.value,
        flag: true,
        id: flowPermission1.value.id
    })
    closeDrawer()
}
const closeDrawer = () => {
    setPromoter(false)
}
</script>
<style scoped lang="scss">
@use "@/assets/styles/antflow/dialog.scss";

.set_promoter {
    .promoter_content {
        padding: 0 20px;

        .el-button {
            margin-bottom: 20px;
        }

        p {
            padding: 18px 0;
            font-size: 14px;
            line-height: 20px;
            color: #000000;
        }
    }
}
</style>