<template>
    <img v-if="imgSrc" class="tabs-header-img" :src="imgSrc" alt="tab-img" />
</template>

<script setup>
import { computed } from 'vue'
const props = defineProps({
    processState: {
        type: [Number, String],
        required: true
    }
})

const stateImgMap = {
    1: new URL('@/assets/images/antflow/flow-in.svg', import.meta.url).href,
    2: new URL('@/assets/images/antflow/flow-pass.svg', import.meta.url).href,
    3: new URL('@/assets/images/antflow/flow-cancel.svg', import.meta.url).href,
    6: new URL('@/assets/images/antflow/flow-refuse.svg', import.meta.url).href,
    default: new URL('@/assets/images/antflow/flow-end.svg', import.meta.url).href
}

const imgSrc = computed(() => {
    return stateImgMap[Number(props.processState)] || stateImgMap.default
})
</script>

<style lang="scss" scoped>
.tabs-header-img {
    position: absolute;
    top: 0;
    right: 0;
    width: 100px;
    height: 100px;
    z-index: 10;
    pointer-events: none;
}
</style>
