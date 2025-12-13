<template>
    <div ref="chartDom" style="height:400px"></div>
</template>
<script setup>
import { ref, onMounted } from 'vue'
import * as echarts from 'echarts';
const chartDom = ref(null);
onMounted(() => {
    const chart = echarts.init(chartDom.value);
    chart.setOption({
        xAxis: { type: 'category' },
        yAxis: { type: 'value' },
        series: [{ data: [120, 200, 150], type: 'line' }]
    });
    window.addEventListener('resize', () => chart.resize()); // 响应式
});
</script>