<!--
 * @Author: lidonghui
 * @Date: 2024-11-09 12:09:41
 * @LastEditTime: 2024-11-09 12:09:41
 * @FilePath: \components\ESign\index.vue
-->
<template>
  <el-dialog title="电子签名" v-model="visible" style="width: 950px !important;min-height: 450px;" append-to-body> 
      <el-row style="min-height: 260px;" :gutter="5">
        <el-col :span="15">
          <div style="border: 1px solid rgb(236 236 236); height: 250px;  width: 560px;">
            <e-sign
              ref="esignRef"
              :width="560"
              :height="250"
              :image="props.image"
              :is-crop="options.isCrop"
              :line-width="options.lineWidth"
              :line-color="options.lineColor"
              v-model:bg-color="options.bgColor"
            />
          </div>
        </el-col>
        <el-col :span="9">
          <div style="width: auto; height: 90px">
            <img :src="resultImg" style="width: 100%; height: 90px; border: 1px solid rgb(236 236 236)" />
          </div>
        </el-col>
      </el-row>
      <div style="margin-top: 10px">
        <el-space>
          <el-form>
            <el-row :gutter="16">
              <el-col :span="12">
                <el-form-item label="画笔粗细：">
                  <el-input-number v-model="options.lineWidth" :min="1" :max="20" />
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-button type="primary" @click="handleGenerate">预览</el-button>
                <el-button @click="handleReset">清屏</el-button>
              </el-col>
            </el-row>
          </el-form>
        </el-space>
      </div>
      <template #footer>
        <el-button @click="visible = false"> 取消 </el-button>
        <el-button type="primary" @click="handleSubmit"> 确定 </el-button>
      </template>
  </el-dialog>
</template>

<script setup lang="js"> 
import { ElMessage } from 'element-plus'
import ESign from "@/components/ESign"; 

const visible = ref(false); //是否显示表单
const options = reactive({
  isCrop: false, //是否开启裁剪
  lineWidth: 6, //线条宽度
  lineColor: "#000000", //线条颜色
  bgColor: "" //背景颜色
});

const props = defineProps({
  image: {
    type: String,
    default: ""
  }
});
const emit = defineEmits(["successful"]); 
const esignRef = ref(); //签名容器
const resultImg = ref(props.image); //签名结果
//console.log("props.image======", props.image);
function open() {
  visible.value = true;
}

function handleGenerate() {
  if (esignRef.value) {
    //生成签名
    esignRef.value
      .generate(null)
      .then(res => { 
        resultImg.value = res;
      })
      .catch(() => {
        ElMessage.error("无任何签名");
      });
  }
}

function handleReset() {
  //清空签名
  resultImg.value = "";
  esignRef.value?.reset();
}
function handleSubmit() {
  if (esignRef.value) {
    //生成签名
    esignRef.value
      .generate(null)
      .then(response => {
        //console.log("esign.response==", response);
        emit("successful", response);
        visible.value = false;
      })
      .catch(() => {
        ElMessage.error("无任何签名");
      });
  }
}

defineExpose({
  open
});
</script>

<style lang="scss" scoped></style>
