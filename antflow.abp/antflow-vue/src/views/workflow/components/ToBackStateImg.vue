<template>
    <img v-if="imgSrc" class="tabs-left-header-img" :src="imgSrc" alt="tab-img" />
</template>

<script setup>
import { computed } from 'vue'
const stateImgMap = {
    default: new URL('@/assets/images/antflow/flow-back.svg', import.meta.url).href
}

const imgSrc = computed(() => {
    return stateImgMap.default
})
</script>

<style lang="scss" scoped>
.tabs-left-header-img {
    position: absolute;
    top: 0;
    left: 0;
    width: 70px;
    height: 70px;
    z-index: 10;
    pointer-events: none;
}
</style>
