import router from "@/router";
import { ElMessageBox } from "element-plus";
import Cookies from "js-cookie";
import { login,  } from "@/api/system/login";
 import {  getInfo, logout } from "@/api/workflow/mock";
import { getToken, setToken, removeToken } from "@/utils/auth";
import { isHttp, isEmpty } from "@/utils/validate";
import defAva from "@/assets/images/profile.jpg";

const useUserStore = defineStore("user", {
  state: () => ({
    token: getToken(),
    id: "",
    name: "",
    nickName: "",
    avatar: "",
    roles: [],
    permissions: [],
  }),
  actions: {
    setNickName(nickName) {
      this.nickName = nickName;
    },
    // 登录
    login(userInfo) {
      const username = userInfo.username.trim();
      const password = userInfo.password;
      const code = userInfo.code;
      const uuid = userInfo.uuid;
      return new Promise((resolve, reject) => {
        login(username, password, code, uuid)
          .then((res) => {
            console.log("login response", res);
            setToken(res.accessToken);
            this.token = res.accessToken;
            resolve();
          })
          .catch((error) => {
            reject(error);
          });
      });
    },
    // 获取用户信息
    getInfo() {
      return new Promise((resolve, reject) => {
        getInfo()
          .then((res) => {
            const user = res.user;
            let avatar = user.avatar || "";
            if (!isHttp(avatar)) {
              avatar = isEmpty(avatar)
                ? defAva
                : import.meta.env.VITE_APP_BASE_API + avatar;
            }
            if (res.roles && res.roles.length > 0) {
              // 验证返回的roles是否是一个非空数组
              this.roles = res.roles;
              this.permissions = res.permissions;
            } else {
              this.roles = ["ROLE_DEFAULT"];
            }
            this.id = user.userId;
            this.name = user.userName;
            this.nickName = decodeURIComponent(Cookies.get("userName") ?? ""); //user.nickName;
            this.avatar = avatar;
            /* 初始密码提示 */
            if (res.isDefaultModifyPwd) {
              ElMessageBox.confirm(
                "您的密码还是初始密码，请修改密码！",
                "安全提示",
                {
                  confirmButtonText: "确定",
                  cancelButtonText: "取消",
                  type: "warning",
                }
              )
                .then(() => {
                  router.push({
                    name: "Profile",
                    params: { activeTab: "resetPwd" },
                  });
                })
                .catch(() => {});
            }
            /* 过期密码提示 */
            if (!res.isDefaultModifyPwd && res.isPasswordExpired) {
              ElMessageBox.confirm(
                "您的密码已过期，请尽快修改密码！",
                "安全提示",
                {
                  confirmButtonText: "确定",
                  cancelButtonText: "取消",
                  type: "warning",
                }
              )
                .then(() => {
                  router.push({
                    name: "Profile",
                    params: { activeTab: "resetPwd" },
                  });
                })
                .catch(() => {});
            }
            resolve(res);
          })
          .catch((error) => {
            reject(error);
          });
      });
    },
    // 退出系统
    logOut() {
      return new Promise((resolve, reject) => {
        logout(this.token)
          .then(() => {
            this.token = "";
            this.roles = [];
            this.permissions = [];
            removeToken();
            resolve();
          })
          .catch((error) => {
            reject(error);
          });
      });
    },
  },
});

export default useUserStore;
