<template>
   <div class="app-container">
      <el-row :gutter="20">
         <el-col :span="6" :xs="24">
            <el-card class="box-card">
               <template v-slot:header>
                  <div class="clearfix">
                     <span>个人信息</span>
                  </div>
               </template>
               <div>
                  <div class="text-center">
                     <userAvatar />
                  </div>
                  <ul class="list-group list-group-striped">
                     <li class="list-group-item">
                        <svg-icon icon-class="user" />用户名称
                        <div class="pull-right">{{ _userName }}</div>
                     </li>
                     <li class="list-group-item">
                        <svg-icon icon-class="phone" />手机号码
                        <div class="pull-right">{{ state.user.phonenumber }}</div>
                     </li>
                     <li class="list-group-item">
                        <svg-icon icon-class="email" />用户邮箱
                        <div class="pull-right">{{ state.user.email }}</div>
                     </li>
                     <li class="list-group-item">
                        <svg-icon icon-class="tree" />所属部门
                        <div class="pull-right" v-if="state.user.dept">{{ state.user.dept.deptName }} / {{
                           state.postGroup }}</div>
                     </li>
                     <li class="list-group-item">
                        <svg-icon icon-class="peoples" />所属角色
                        <div class="pull-right">{{ state.roleGroup }}</div>
                     </li>
                     <li class="list-group-item">
                        <svg-icon icon-class="date" />创建日期
                        <div class="pull-right">{{ state.user.createTime }}</div>
                     </li>
                  </ul>
                  <div class="account-center-team">
                     <div v-if="state.signature" class="mb-2" style="width: 100%">
                        <el-image :src="state.signature" style="height: 120px; border: 1px solid rgb(236 236 236)"
                           width="100%" />
                     </div>
                     <el-button @click="openSignName">打开签名板</el-button>
                  </div>
               </div>
            </el-card>
         </el-col>
         <el-col :span="18" :xs="24">
            <el-card>
               <template v-slot:header>
                  <div class="clearfix">
                     <span>基本资料</span>
                  </div>
               </template>
               <el-tabs v-model="activeTab">
                  <el-tab-pane label="基本资料" name="userinfo">
                     <userInfo :user="state.user" />
                  </el-tab-pane>
                  <el-tab-pane label="修改密码" name="resetPwd">
                     <resetPwd />
                  </el-tab-pane>
               </el-tabs>
            </el-card>
         </el-col>
      </el-row>
      <sign-name ref="signNameRef" v-if="state.signature" :image="state.signature"
         @successful="signSuccess"></sign-name>
   </div>
</template>

<script setup name="Profile">
import Cookies from "js-cookie";
import userAvatar from "./userAvatar";
import userInfo from "./userInfo";
import resetPwd from "./resetPwd";
import SignName from "./signName.vue";
//import { getUserProfile } from "@/api/system/user";
import { getUserProfile, getSignatureData } from "@/api/workflow/mock";

import useUserStore from "@/store/modules/user";
const userStore = useUserStore();

const _userName = decodeURIComponent(Cookies.get('userName'));
const activeTab = ref("userinfo");
const state = reactive({
   user: {},
   roleGroup: {},
   postGroup: {},
   signature: ''
});

function getUser() {
   getUserProfile().then(response => {
      state.user = response.data;
      state.roleGroup = response.roleGroup;
      state.postGroup = response.postGroup;
   });
   getSignatureData().then(response => {
      state.signature = response.data;
   });
};
getUser();

const signNameRef = ref();
/** 打开签名板 */
function openSignName() {
   signNameRef.value.open();
}

/** 签名成功回调 */
function signSuccess(url) {
   const param = {
      signature: url
   };
   state.signature = url;
   userStore.setSignature(url);
   //   userCenterApi.updateSignature(param).then(() => {
   //     userStore.setUserInfoItem("signature", url);
   //   });

   //console.log("userStore====.signature===============", userStore.signature);
}

</script>
<style lang="scss" scoped>
.account-center-team {
   .members {
      a {
         display: block;
         height: 24px;
         margin: 12px 0;
         line-height: 24px;

         .member {
            display: inline-block;
            max-width: 100px;
            margin-left: 12px;
            font-size: 14px;
            line-height: 24px;
            vertical-align: top;
            transition: all 0.3s;
         }
      }
   }
}
</style>