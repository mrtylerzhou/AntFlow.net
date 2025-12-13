<template>
    <div class="app-container">
        <el-tabs :tab-position="tabPosition" v-model="activeName">
            <el-tab-pane label="通用模板" name="comTemplate">
                <com-template-list v-if="activeName == 'comTemplate'" />
            </el-tab-pane>
            <el-tab-pane label="短信模板" name="smsTemplate">
                <sms-template-list v-if="activeName == 'smsTemplate'" />
            </el-tab-pane>
        </el-tabs>
    </div>
</template>

<script setup>
import comTemplateList from "./templates/comTemplateList.vue";
import smsTemplateList from "./templates/smsTemplateList.vue";
const tabPosition = ref('left')
const activeName = ref('comTemplate')
</script>
<style scoped>
::v-deep(.el-tabs__item.is-left.is-active) {
    background-color: var(--current-color) !important;
    color: var(--navbar-bg);
}

::v-deep(.el-tabs__header) {
    min-height: 300px;
}

::v-deep(.el-tabs__header) {
    background-color: var(--navbar-bg) !important;
    font-size: 18px;
}
</style>