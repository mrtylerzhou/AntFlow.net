<!--
 * @Author: lidonghui
 * @Date: 2024-11-09 12:09:41
 * @LastEditTime: 2024-11-09 12:09:41
 * @FilePath: \components\ESign\index.vue
-->
<template>
  <canvas
    ref="canvasRef"
    @mousedown="mouseDown"
    @mousemove="mouseMove"
    @mouseup="mouseUp"
    @touchstart="touchStart"
    @touchmove="touchMove"
    @touchend="touchEnd"
  ></canvas>
</template>

<script setup lang="js" name="ESign"> 
const canvasRef = ref(null);

const esignProps = reactive({
  hasDrew: false, // 是否已经绘制
  resultImg: "", // 绘制结果图片
  points: [], // 绘制的点集合
  canvasTxt: null, // 画布上下文
  startX: 0, // 绘制起始点的X坐标
  startY: 0, // 绘制起始点的Y坐标
  isDrawing: false, // 是否正在绘制
  sratio: 1 // 缩放比例
});
 
const props = defineProps({
  width: {// 画布宽度
    type: Number,
    default: 800
  },
  height: {// 画布高度
    type: Number,
    default: 300
  },
  isCrop: {// 是否裁剪
    type: Boolean,
    default: false
  },
  lineWidth: {// 线条宽度
    type: Number,
    default: 4
  },
  lineColor: {// 线条颜色
    type: String,
    default: "#000000"
  },
  bgColor: {// 背景颜色
    type: String,
    default: "#000000"
  },
  isClearBgColor: {// 是否清除背景颜色
    type: Boolean,
    default:true
  },
  format: {// 图片格式
    type: String,
    default:"image/png"
  },
  quality: {// 图片质量
    type: Number,
    default: 1 
  }
});

const emit = defineEmits(["update:bgColor"]); // 定义组件的事件

const ratio = computed(() => {
  return props.height / props.width;
});

const myBg = computed(() => {
  return props.bgColor ? props.bgColor : "rgba(255, 255, 255, 0)";
});

onBeforeMount(() => {
  window.addEventListener("resize", resizeHandler);
});

onBeforeUnmount(() => {
  window.removeEventListener("resize", resizeHandler);
});

onMounted(() => {
  if (canvasRef.value) {
    const canvas = canvasRef.value;
    canvas.height = props.height;
    canvas.width = props.width;
    canvas.style.background = myBg.value;
    resizeHandler();
    // 在画板以外松开鼠标后冻结画笔
    document.onmouseup = () => {
      esignProps.isDrawing = false;
    };
  }
});

/**
 * 处理画布元素的调整大小。
 */
function resizeHandler() {
  if (canvasRef.value) {
    const canvas = canvasRef.value;
    canvas.style.width = props.width + "px";
    const realw = parseFloat(window.getComputedStyle(canvas).width);
    canvas.style.height = ratio.value * realw + "px";
    // 根据实际宽度计算比率
    esignProps.sratio = realw / props.width;
    // 获取画布上下文
    esignProps.canvasTxt = canvas.getContext("2d");
    if (esignProps.canvasTxt) {
      // 在应用新的比例之前重置缩放，以避免累积
      esignProps.canvasTxt.setTransform(1, 0, 0, 1, 0, 0);
      // 应用新的比例
      esignProps.canvasTxt.scale(1 * esignProps.sratio, 1 * esignProps.sratio);
    }
  }
}

/** 鼠标按下 */
function mouseDown(e) {
  esignProps.isDrawing = true;
  esignProps.hasDrew = true;
  let obj = {
    x: e.offsetX,
    y: e.offsetY
  };
  drawStart(obj);
}

function mouseMove(e) {
  if (esignProps.isDrawing) {
    let obj = {
      x: e.offsetX,
      y: e.offsetY
    };
    drawMove(obj);
  }
}

function mouseUp(e) {
  let obj = {
    x: e.offsetX,
    y: e.offsetY
  };
  drawEnd(obj);
  esignProps.isDrawing = false;
}

function touchStart(e) {
  esignProps.hasDrew = true;
  if (e.touches.length === 1) {
    if (canvasRef.value) {
      let obj = {
        x: e.targetTouches[0].clientX - canvasRef.value.getBoundingClientRect().left,
        y: e.targetTouches[0].clientY - canvasRef.value.getBoundingClientRect().top
      };
      drawStart(obj);
    }
  }
}

function touchMove(e) {
  if (e.touches.length === 1) {
    if (canvasRef.value) {
      let obj = {
        x: e.targetTouches[0].clientX - canvasRef.value.getBoundingClientRect().left,
        y: e.targetTouches[0].clientY - canvasRef.value.getBoundingClientRect().top
      };
      drawMove(obj);
    }
  }
}

function touchEnd(e) {
  if (e.touches.length === 1) {
    if (canvasRef.value) {
      let obj = {
        x: e.targetTouches[0].clientX - canvasRef.value.getBoundingClientRect().left,
        y: e.targetTouches[0].clientY - canvasRef.value.getBoundingClientRect().top
      };
      drawEnd(obj);
    }
  }
}

function drawStart(obj) {
  esignProps.startX = obj.x;
  esignProps.startY = obj.y;
  if (esignProps.canvasTxt) {
    esignProps.canvasTxt.beginPath();
    esignProps.canvasTxt.moveTo(esignProps.startX, esignProps.startY);
    esignProps.canvasTxt.lineTo(obj.x, obj.y);
    esignProps.canvasTxt.lineCap = "round";
    esignProps.canvasTxt.lineJoin = "round";
    esignProps.canvasTxt.lineWidth = props.lineWidth * esignProps.sratio;
    esignProps.canvasTxt.stroke();
    esignProps.canvasTxt.closePath();
    esignProps.points.push(obj);
  }
}

function drawMove(obj) {
  if (esignProps.canvasTxt) {
    esignProps.canvasTxt.beginPath();
    esignProps.canvasTxt.moveTo(esignProps.startX, esignProps.startY);
    esignProps.canvasTxt.lineTo(obj.x, obj.y);
    esignProps.canvasTxt.strokeStyle = props.lineColor;
    esignProps.canvasTxt.lineWidth = props.lineWidth * esignProps.sratio;
    esignProps.canvasTxt.lineCap = "round";
    esignProps.canvasTxt.lineJoin = "round";
    esignProps.canvasTxt.stroke();
    esignProps.canvasTxt.closePath();
    esignProps.startY = obj.y;
    esignProps.startX = obj.x;
    esignProps.points.push(obj);
  }
}

function drawEnd(obj) {
  if (esignProps.canvasTxt) {
    esignProps.canvasTxt.beginPath();
    esignProps.canvasTxt.moveTo(esignProps.startX, esignProps.startY);
    esignProps.canvasTxt.lineTo(obj.x, obj.y);
    esignProps.canvasTxt.lineCap = "round";
    esignProps.canvasTxt.lineJoin = "round";
    esignProps.canvasTxt.stroke();
    esignProps.canvasTxt.closePath();
    esignProps.points.push(obj);
    esignProps.points.push({ x: -1, y: -1 });
  }
}

function generate(options) {
  let imgFormat = options && options.format ? options.format : props.format;
  let imgQuality = options && options.quality ? options.quality : props.quality;
  const pm = new Promise((resolve, reject) => {
    if (!esignProps.hasDrew) {
      reject(`Warning: Not Signned!`);
      return;
    }
    if (esignProps.canvasTxt && canvasRef.value) {
      esignProps.canvasTxt.willReadFrequently = true;
      let resImgData = esignProps.canvasTxt.getImageData(0, 0, canvasRef.value.width, canvasRef.value.height, {willReadFrequently: true});
      esignProps.canvasTxt.globalCompositeOperation = "destination-over";
      esignProps.canvasTxt.fillStyle = myBg.value;
      esignProps.canvasTxt.fillRect(0, 0, canvasRef.value.width, canvasRef.value.height);
      esignProps.resultImg = canvasRef.value.toDataURL(imgFormat, imgQuality);
      let resultImg= esignProps.resultImg;
      esignProps.canvasTxt.clearRect(0, 0, canvasRef.value.width, canvasRef.value.height);
      esignProps.canvasTxt.putImageData(resImgData, 0, 0);
      esignProps.canvasTxt.globalCompositeOperation = "source-over";
      if (props.isCrop) {
        const crop_area = getCropArea(resImgData.data);
        let crop_canvas= document.createElement("canvas");
        const crop_ctx = crop_canvas.getContext("2d");
        if (crop_ctx && crop_area) {
          crop_canvas.width = crop_area[2] - crop_area[0];
          crop_canvas.height = crop_area[3] - crop_area[1];
          console.log("[ crop_area ] >", crop_area);
          const crop_imgData = esignProps.canvasTxt.getImageData(crop_area[0], crop_area[1], crop_area[2], crop_area[3], {willReadFrequently: true});
          crop_ctx.globalCompositeOperation = "destination-over";
          crop_ctx.putImageData(crop_imgData, 0, 0);
          crop_ctx.fillStyle = this.myBg;
          crop_ctx.fillRect(0, 0, crop_canvas.width, crop_canvas.height);
          resultImg = crop_canvas.toDataURL(imgFormat, imgQuality);
          crop_canvas = null;
        }
      }
      resolve(resultImg);
    }
  });
  return pm;
}

function reset() {
  if (esignProps.canvasTxt && canvasRef.value) {
    esignProps.canvasTxt.clearRect(0, 0, canvasRef.value.width, canvasRef.value.height);
    if (props.isClearBgColor) {
      emit("update:bgColor", "");
      canvasRef.value.style.background = "rgba(255, 255, 255, 0)";
    }
    esignProps.points = [];
    esignProps.hasDrew = false;
    esignProps.resultImg = "";
  }
}

function getCropArea(imgData) {
  if (canvasRef.value) {
    let topX = canvasRef.value.width;
    let btmX = 0;
    let topY = canvasRef.value.height;
    let btnY = 0;
    for (let i = 0; i < canvasRef.value.width; i++) {
      for (let j = 0; j < canvasRef.value.height; j++) {
        let pos = (i + canvasRef.value.width * j) * 4;
        if (imgData[pos] > 0 || imgData[pos + 1] > 0 || imgData[pos + 2] || imgData[pos + 3] > 0) {
          btnY = Math.max(j, btnY);
          btmX = Math.max(i, btmX);
          topY = Math.min(j, topY);
          topX = Math.min(i, topX);
        }
      }
    }
    topX++;
    btmX++;
    topY++;
    btnY++;
    const data = [topX, topY, btmX, btnY];
    return data;
  }
}

defineExpose({
  generate,
  reset
});
</script>

<style lang="scss" scoped>
canvas {
  display: block;
  max-width: 100%;
}
</style>
