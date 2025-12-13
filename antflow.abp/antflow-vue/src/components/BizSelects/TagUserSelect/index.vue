<template>
    <div class="tag-select" :class="(selectValues && selectValues.length < 1 ? 'validate-class' : '')">
        <div class="tag-box" v-if="selectValues && selectValues.length > 0">
            <el-tag v-for="item in selectValues" :key="item.id" effect="dark" style="margin-right: 5px" type="primary"
                :size="layoutSize" closable @close="onDeleteTag(item)">
                {{ item.name }}
            </el-tag>
        </div>
        <div class="tag-box" v-else>
            <span style="margin-left: 5px;color: #a8abb2">{{placeholder}}</span>
        </div>
        <el-button class="append-add" type="default" icon="Plus" @click="userDialogVisible = true" />
        <selectUser v-if="userDialogVisible" v-model:visible="userDialogVisible" v-model:checkedData="selectValues" :multiple="multiple"
            @change="saveUserDialog" />
    </div>
</template>

<script setup>
import { ref, watch } from 'vue';
import selectUser from './userListDialog.vue';
const emits = defineEmits(['update:list']);
const props = defineProps({
    placeholder: {
        type: String,
        default: '请选择人员'
    },
    multiple: {
        type: Boolean,
        default: false
    },
    list: {
        type: Array,
        default: []
    }
})
let userDialogVisible = ref(false);
const layoutSize = ref('default');
const selectValues = ref(null); 
const onDeleteTag = (tag) => {
    let temp = selectValues.value.filter(item => {
        return item.id !== tag.id;
    });
    if (Array.isArray(props.list)) {
        emits('update:list', temp);
    } else {
        emits('update:list', temp.map(item => item.id).join(','));
    }
};

watch(() => props.list, newValue => {
    if (newValue == null || newValue === '') {
        selectValues.value = [];
    } else {
        if (!Array.isArray(newValue) || newValue.length < 0) {
            return;
        }
        selectValues.value = [...newValue];
    }
}, { deep: true });

const saveUserDialog = (data) => {
    emits('update:list', data);
}
 
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

.validate-class {
    box-shadow: 0 0 0 1px #ff4949;
}
</style>