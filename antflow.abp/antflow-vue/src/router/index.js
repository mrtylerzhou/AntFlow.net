import { createWebHashHistory, createRouter } from "vue-router";
/* Layout */
import Layout from "@/layout";

/**
 * Note: 路由配置项
 *
 * hidden: true                     // 当设置 true 的时候该路由不会再侧边栏出现 如401，login等页面，或者如一些编辑页面/edit/1
 * alwaysShow: true                 // 当你一个路由下面的 children 声明的路由大于1个时，自动会变成嵌套的模式--如组件页面
 *                                  // 只有一个时，会将那个子路由当做根路由显示在侧边栏--如引导页面
 *                                  // 若你想不管路由下面的 children 声明的个数都显示你的根路由
 *                                  // 你可以设置 alwaysShow: true，这样它就会忽略之前定义的规则，一直显示根路由
 * redirect: noRedirect             // 当设置 noRedirect 的时候该路由在面包屑导航中不可被点击
 * name:'router-name'               // 设定路由的名字，一定要填写不然使用<keep-alive>时会出现各种问题
 * query: '{"id": 1, "name": "ry"}' // 访问路由的默认传递参数
 * roles: ['admin', 'common']       // 访问路由的角色权限
 * permissions: ['a:a:a', 'b:b:b']  // 访问路由的菜单权限
 * meta : {
    noCache: true                   // 如果设置为true，则不会被 <keep-alive> 缓存(默认 false)
    title: 'title'                  // 设置该路由在侧边栏和面包屑中展示的名字
    icon: 'svg-name'                // 设置该路由的图标，对应路径src/assets/icons/svg
    breadcrumb: false               // 如果设置为false，则不会在breadcrumb面包屑中显示
    activeMenu: '/system/user'      // 当路由设置了该属性，则会高亮相对应的侧边栏。
  }
 */

// 公共路由
export const constantRoutes = [
  {
    path: "/redirect",
    component: Layout,
    hidden: true,
    children: [
      {
        path: "/redirect/:path(.*)",
        component: () => import("@/views/redirect/index.vue"),
      },
    ],
  },
  {
    path: "/login",
    component: () => import("@/views/login"),
    hidden: true,
  },
  {
    path: "/:pathMatch(.*)*",
    component: () => import("@/views/error/404"),
    hidden: true,
  },
  {
    path: "/401",
    component: () => import("@/views/error/401"),
    hidden: true,
  },
  {
    path: "/flowMsg/msgTemp",
    component: () => import("@/views/workflow/flowMsg/msgTemplete/index.vue"),
    hidden: false,
  },
  {
    path: "",
    component: Layout,
    redirect: "/index",
    children: [
      {
        path: "/index",
        component: () => import("@/views/index"),
        name: "Index",
        meta: { title: "首页", icon: "dashboard", affix: true },
      },
    ],
  },
  {
    path: "/user",
    component: Layout,
    hidden: true,
    redirect: "noredirect",
    children: [
      {
        path: "profile",
        component: () => import("@/views/system/user/profile/index"),
        name: "Profile",
        meta: { title: "个人中心", icon: "user" },
      },
    ],
  },
  {
    path: "/system",
    name: "system",
    hidden: false,
    component: () => import("@/views/system/user/index.vue"),
  },
];

// 动态路由，基于用户权限动态去加载
export const dynamicRoutes = [
  {
    path: "/system/user-auth",
    component: Layout,
    hidden: true,
    permissions: ["system:user:edit"],
    children: [
      {
        path: "role/:userId(\\d+)",
        component: () => import("@/views/system/user/authRole"),
        name: "AuthRole",
        meta: { title: "分配角色", activeMenu: "/system/user" },
      },
    ],
  },
  {
    path: "/startFlow/index",
    component: Layout,
    hidden: true,
    permissions: ["system"],
    children: [
      {
        path: "/startFlow/index",
        component: () => import("@/views/workflow/startFlow/index"),
        name: "startFlow",
        meta: {
          title: "发起流程申请",
          activeMenu: "/taskCenter",
        },
      },
    ],
  },
  {
    path: "/startOutside/index",
    component: Layout,
    hidden: true,
    permissions: ["system"],
    children: [
      {
        path: "/startOutside/index",
        component: () => import("@/views/workflow/startOutside/index"),
        name: "startOutside",
        meta: {
          title: "Saas流程申请",
          activeMenu: "/taskCenter",
        },
      },
    ],
  },
  {
    path: "/workflow/diy-design",
    component: Layout,
    hidden: true,
    permissions: ["system"],
    children: [
      {
        path: "/workflow/diy-design",
        component: () => import("@/views/workflow/flowDesign/diy"),
        name: "diy-design",
        meta: {
          title: "DIY流程设计",
          activeMenu: "/workflow/flowList",
        },
      },
    ],
  },
  {
    path: "/workflow/lf-design",
    component: Layout,
    hidden: true,
    permissions: ["system"],
    children: [
      {
        path: "/workflow/lf-design",
        component: () => import("@/views/workflow/flowDesign/lf"),
        name: "lf-design",
        meta: {
          title: "LF流程设计",
          activeMenu: "/workflow/flowList",
        },
      },
    ],
  },
  {
    path: "/workflow/flow-version",
    component: Layout,
    hidden: true,
    permissions: ["system"],
    children: [
      {
        path: "/workflow/flow-version",
        component: () => import("@/views/workflow/flowList/version"),
        name: "version",
        meta: {
          title: "版本管理",
          activeMenu: "/workflow/flowList",
        },
      },
    ],
  },
  {
    path: "/workflow/flowPreview",
    component: Layout,
    hidden: true,
    permissions: ["system"],
    children: [
      {
        path: "/workflow/flowPreview",
        component: () => import("@/views/workflow/flowPreview/index"),
        name: "flowPreview",
        meta: {
          title: "流程预览",
          activeMenu: "/workflow/flowList",
        },
      },
    ],
  },
  {
    path: "/outsideMgt/preview",
    component: Layout,
    hidden: true,
    permissions: ["system"],
    children: [
      {
        path: "/outsideMgt/preview",
        component: () => import("@/views/workflow/outsideMgt/preview/index"),
        name: "outsideFlowPreview",
        meta: {
          title: "Saas流程预览",
          activeMenu: "/outsideMgt/outsideTemp",
        },
      },
    ],
  },
  {
    path: "/outsideMgt/outsideDesign",
    component: Layout,
    hidden: true,
    permissions: ["system"],
    children: [
      {
        path: "/outsideMgt/outsideDesign",
        component: () =>
          import("@/views/workflow/outsideMgt/outsideDesign/index"),
        name: "outsideFlowDesign",
        meta: {
          title: "Saas流程复制",
          activeMenu: "/outsideMgt/outsideTemp",
        },
      },
    ],
  },
  {
    path: "/outsideMgt/app-setting",
    component: Layout,
    hidden: true,
    permissions: ["system"],
    children: [
      {
        path: "/outsideMgt/app-setting",
        component: () =>
          import("@/views/workflow/outsideMgt/outsideApp/Setting/index"),
        name: "app-setting",
        meta: {
          title: "应用配置",
          activeMenu: "/outsideMgt/outsideApp",
        },
      },
    ],
  },
  {
    path: "/workflow/instance/removeSign",
    component: Layout,
    hidden: true,
    permissions: ["system"],
    children: [
      {
        path: "processNumber/:processNumber",
        component: () =>
          import("@/views/workflow/flowTask/instance/flowRemoveSign"),
        name: "removeSign",
        meta: { title: "流程减签", activeMenu: "/workflow/instance" },
      },
    ],
  },
  {
    path: "/workflow/instance/addSign",
    component: Layout,
    hidden: true,
    permissions: ["system"],
    children: [
      {
        path: "processNumber/:processNumber",
        component: () =>
          import("@/views/workflow/flowTask/instance/flowAddSign"),
        name: "addSign",
        meta: { title: "流程加签", activeMenu: "/workflow/instance" },
      },
    ],
  },
  {
    path: "/workflow/instance/changeSign",
    component: Layout,
    hidden: true,
    permissions: ["system"],
    children: [
      {
        path: "processNumber/:processNumber",
        component: () =>
          import("@/views/workflow/flowTask/instance/flowChangeSign"),
        name: "changeSign",
        meta: { title: "变更审批人", activeMenu: "/workflow/instance" },
      },
    ],
  },
  {
    path: "/flowDevOps/flowPrint",
    hidden: true,
    permissions: ["system"],
    children: [
      {
        path: "/flowDevOps/flowPrint",
        component: () => import("@/views/workflow/flowPrint/index"),
        name: "flowPrint",
        meta: { title: "流程打印预览" },
      },
    ],
  },
];

const router = createRouter({
  history: createWebHashHistory(import.meta.env.BASE_URL), //createWebHistory(), //
  routes: constantRoutes,
  scrollBehavior(to, from, savedPosition) {
    if (savedPosition) {
      return savedPosition;
    } else {
      return { top: 0 };
    }
  },
});

export default router;
