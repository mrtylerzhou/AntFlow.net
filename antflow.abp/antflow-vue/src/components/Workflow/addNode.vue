<template>
    <div class="add-node-btn-box">
        <div class="add-node-btn">
            <el-popover placement="right-start" v-model="visible" aria-hidden="true" width="auto">
                <div class="add-node-popover-body">
                    <a class="add-node-popover-item approver" @click="addType(1)">
                        <div class="item-wrapper">
                            <svg-icon icon-class="approve" class="iconfont" />
                            <p>审批人</p>
                        </div>
                    </a>
                    <a class="add-node-popover-item approver" @click="addType(3)">
                        <div class="item-wrapper">
                            <svg-icon icon-class="parallel-approve" class="iconfont" />
                            <p>并行审批</p>
                        </div>
                    </a>
                    <a class="add-node-popover-item notifier" @click="addType(2)">
                        <div class="item-wrapper">
                            <svg-icon icon-class="copy-user" class="iconfont" />
                            <p>抄送人</p>
                        </div>
                    </a>
                </div>
                <div class="add-node-popover-body">
                    <a class="add-node-popover-item condition" @click="addType(4)">
                        <div class="item-wrapper">
                            <svg-icon icon-class="condition" class="iconfont" />
                            <p>条件分支</p>
                        </div>
                    </a>
                    <a class="add-node-popover-item condition" @click="addType(5)">
                        <div class="item-wrapper">
                            <svg-icon icon-class="dynamic-condition" class="iconfont" />
                            <p>动态条件</p>
                        </div>
                    </a>
                    <a class="add-node-popover-item condition" @click="addType(6)">
                        <div class="item-wrapper">
                            <svg-icon icon-class="parallel-condition" class="iconfont" />
                            <p>条件并行</p>
                        </div>
                    </a>
                </div>
                <template #reference>
                    <button class="btn" type="button">
                        <svg-icon icon-class="addbtn" class="iconfont" />
                    </button>
                </template>
            </el-popover>
        </div>
    </div>
</template>
<script setup>
import { ref } from 'vue'
import { NodeUtils } from '@/utils/antflow/nodeUtils'
let props = defineProps({
    childNodeP: {
        type: Object,
        default: () => (null)
    }
})
let emits = defineEmits(['update:childNodeP'])
let visible = ref(false)
/**创建审批人节点 */
const createApproveNode = (childNode) => {
    return NodeUtils.createApproveNode(childNode);
}
/**创建抄送人节点 */
const createCopyNode = (childNode) => {
    return NodeUtils.createCopyNode(childNode);
}
/**创建并行审批人节点 */
const createParallelWayNode = (childNode) => {
    return NodeUtils.createParallelWayNode(childNode);
}
/**创建条件网关节点 */
const createGatewayNode = (childNode) => {
    return NodeUtils.createGatewayNode(childNode);
}
/**创建动态网关节点 */
const createDynamicConditionWayNode = (childNode) => {
    return NodeUtils.createDynamicConditionWayNode(childNode);
}
/**创建并行网关节点 */
const createParallelConditionWayNode = (childNode) => {
    return NodeUtils.createParallelConditionWayNode(childNode);
}
// 创建节点 Map集合
const createNodeMap = new Map([
    [1, createApproveNode],
    [2, createCopyNode],
    [3, createParallelWayNode],
    [4, createGatewayNode],
    [5, createDynamicConditionWayNode],
    [6, createParallelConditionWayNode]
]);
const addType = (type) => {
    visible.value = false;
    const handleCreateNodeFunc = createNodeMap.get(type);
    const newNodeInfo = handleCreateNodeFunc(props.childNodeP);
    emits("update:childNodeP", newNodeInfo)
} 
</script>
<style scoped lang="scss">
@use "@/assets/styles/antflow/workflow.scss";

.add-node-btn-box {
    width: 240px;
    display: -webkit-inline-box;
    display: -ms-inline-flexbox;
    display: inline-flex;
    -ms-flex-negative: 0;
    flex-shrink: 0;
    -webkit-box-flex: 1;
    -ms-flex-positive: 1;
    position: relative;

    &:before {
        content: "";
        position: absolute;
        top: 0;
        left: 0;
        right: 0;
        bottom: 0;
        z-index: -1;
        margin: auto;
        width: 2px;
        height: 100%;
        background-color: #cacaca
    }

    .add-node-btn {
        user-select: none;
        width: 240px;
        padding: 20px 0 32px;
        display: flex;
        -webkit-box-pack: center;
        justify-content: center;
        flex-shrink: 0;
        -webkit-box-flex: 1;
        flex-grow: 1;

        .btn {
            outline: none;
            box-shadow: 0 2px 4px 0 rgba(0, 0, 0, .1);
            width: 30px;
            height: 30px;
            background: #3296fa;
            border-radius: 50%;
            position: relative;
            border: none;
            line-height: 30px;
            -webkit-transition: all .3s cubic-bezier(.645, .045, .355, 1);
            transition: all .3s cubic-bezier(.645, .045, .355, 1);

            .iconfont {
                color: #fff;
                font-size: 16px
            }

            &:hover {
                transform: scale(1.3);
                box-shadow: 0 13px 27px 0 rgba(0, 0, 0, .1)
            }

            &:active {
                transform: none;
                background: #1e83e9;
                box-shadow: 0 2px 4px 0 rgba(0, 0, 0, .1)
            }
        }
    }
}

.add-node-popover-body {
    display: flex;

    .add-node-popover-item {
        margin-right: 10px;
        cursor: pointer;
        text-align: center;
        flex: 1;
        color: #191f25 !important;

        .item-wrapper {
            user-select: none;
            display: inline-block;
            width: 60px;
            height: 66px;
            margin-bottom: 5px;
            background: #fff;
            border: 1px solid #e2e2e2;
            border-radius: 10%;
            transition: all .3s cubic-bezier(.645, .045, .355, 1);

            .iconfont {
                margin-top: 5px;
                font-size: 35px;
                line-height: 65px
            }
        }

        p {
            margin: 0;
            font-size: 12px;
            font-weight: 900;
            color: #000;
        }

        &.approver {
            .item-wrapper {
                color: #ff943e
            }
        }

        &.notifier {
            .item-wrapper {
                color: #3296fa
            }
        }

        &.condition {
            .item-wrapper {
                color: #15bc83
            }
        }

        &:hover {
            .item-wrapper {
                background: #3296fa;
                box-shadow: 0 10px 20px 0 rgba(50, 150, 250, .4)
            }

            .iconfont {
                color: #fff
            }
        }

        &:active {
            .item-wrapper {
                box-shadow: none;
                background: #eaeaea
            }

            .iconfont {
                color: inherit
            }
        }
    }
}
</style>