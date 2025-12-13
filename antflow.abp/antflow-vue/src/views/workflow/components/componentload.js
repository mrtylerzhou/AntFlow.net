import { markRaw } from "vue";
import { bizFormMaps } from "@/utils/antflow/const";
const modules = import.meta.glob("./forms/*.vue"); // 动态引入组件
import FormRender from "@/components/Workflow/DynamicForm/formRender.vue"; // 低代码表单渲染组件
/**
 * 动态加载自定义（DIY）表单组件
 */
export const loadDIYComponent = (flowCode) => {
  return new Promise((resolve, reject) => {
    if (bizFormMaps.has(flowCode)) {
      const componentPath = bizFormMaps.get(flowCode);
      const componentPathVue = `.${componentPath}`;
      const importDybanicVue = modules[componentPathVue];
      importDybanicVue()
        .then((component) => {
          resolve(markRaw(component.default));
        })
        .catch((err) => {
          reject(err);
        });
    } else {
      reject("未定义业务表单组件");
    }
  });
};
/**
 * 动态加载低代码(LF)表单组件
 */
export const loadLFComponent = () => {
  return new Promise((resolve, reject) => {
    resolve(markRaw(FormRender));
  });
};
