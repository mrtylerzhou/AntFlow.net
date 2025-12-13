<template>
    <el-text :type="viewType">
        <el-icon v-if="iconType" :size="sizeValue">
            <component :is="getTypeIcon(iconType)" />
        </el-icon>
    </el-text>
</template>
<script setup>
import { computed, watchEffect } from "vue";
import {
    Message, BellFilled, Comment, Eleme, ChatDotRound, Iphone, Position
} from "@element-plus/icons-vue";
let props = defineProps({
    iconValue: {
        type: [Number, Object], // 支持对象或数字
        default: 0,
    },
    viewValue: {
        type: String,
        default: "info",
    },
    sizeValue: {
        type: [Number, String],
        default: 15,
    },
});
const validTypes = ["primary", "success", "info", "warning", "danger"];
let iconType = computed(() => props.iconValue);
let viewType = computed(() => {
    return validTypes.includes(props.viewValue) ? props.viewValue : "info";
});

watchEffect(() => {
    // console.log("iconValue changed:", props.iconValue);
    // console.log("viewValue changed:", props.viewValue);
});

/** type与icon映射 */
const typeIconMap = {
    1: Eleme,
    2: Message,
    3: BellFilled,
    4: ChatDotRound,
    5: Comment,
    6: Iphone,
    7: Position,
};
/**
 * 根据type返回icon组件
 * @param {string} type
 */
const getTypeIcon = (type) => {
    return typeIconMap[type] || Message;
};
</script>

<style lang="scss" scoped></style>