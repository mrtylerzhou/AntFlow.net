<template>
    <div class="tag-select">
        <div class="tag-box">
            <el-tag v-if="selectValue" type="warning" effect="dark" :size="layoutSize" closable
                :key="selectValue.nodeId" @close="onDeleteTag(selectValue)">
                {{ selectValue.nodeName }}【{{ selectValue.nodeDisplayName }}】
            </el-tag>
        </div>
        <slot name="append" />
    </div>
</template>

<script setup>
import { ref, watch } from 'vue';
const { proxy } = getCurrentInstance();
const emits = defineEmits(['update:value', 'update:flowNode']);
const props = defineProps({
    value: {
        type: String,
        default: '',
    },
    flowNode: {
        type: Object,
        default: () => undefined
    }
})
const layoutSize = ref('default');
const selectValue = ref(null);

const onDeleteTag = (tag) => {
    emits('update:value', null);
};
watch(() => props.flowNode, newValue => {
    if (!proxy.isEmpty(newValue) && !proxy.isEmpty(newValue.nodeId)) {
        selectValue.value = newValue;
    }
});

watch(() => props.value, newValue => {
    if (!proxy.isEmpty(newValue) && !proxy.isEmpty(newValue.nodeId)) {
        selectValue.value = props.flowNode;
    } else {
        selectValue.value = null;
    }
}, { immediate: true, deep: true, });

</script>

<style scoped lang="scss">
.tag-select {
    display: flex;
    width: 100%;
    border: 1px solid #dcdfe6;
    border-radius: 4px;
}

.tag-box {
    flex-grow: 1;
    padding: 0 5px;
}
</style>