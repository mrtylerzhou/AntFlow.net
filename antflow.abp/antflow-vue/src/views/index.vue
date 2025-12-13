<template>
  <div class="app-container home">
    <shortcuts />
    <el-divider />
    <el-row :gutter="10">
      <el-col :xs="24" :sm="24" :md="12" :lg="12">
        <el-card class="update-log">
          <template v-slot:header>
            <div class="clearfix">
              <span>流程后台管理框架</span>
            </div>
          </template>
          <div class="body">
            <p>
              <span>
                {{ readmeText }}
              </span>
            </p>
            <p>
              <b>当前版本:</b> <span>v{{ version }}</span>
            </p>
            <p>
              <el-tag type="danger">&yen;免费开源</el-tag>
            </p>
            <p>
              <el-row :gutter="5">
                <el-col :xs="12" :sm="12" :md="8" :lg="8">
                  <el-button type="primary" icon="Cloudy" plain style="width: 100%;"
                    @click="goTarget('https://gitee.com/tylerzhou')">访问码云</el-button>
                </el-col>
                <el-col :xs="12" :sm="12" :md="8" :lg="8">
                  <el-button type="primary" icon="Cloudy" plain style="width: 100%;"
                    @click="goTarget('https://gitee.com/ldhnet/AntFlow-Designer')">流程设计器</el-button>
                </el-col>
              </el-row>
            </p>
          </div>
        </el-card>
      </el-col>

      <el-col :xs="24" :sm="24" :md="12" :lg="12">
        <el-card class="update-log">
          <template v-slot:header>
            <div class="clearfix">
              <span>联系信息</span>
            </div>
          </template>
          <div class="body">
            <p>
              <i class="el-icon-s-promotion"></i> 官网：<el-link href="http://antflow.top"
                target="_blank">http://antflow.top</el-link>
            </p>
            <p>
              <i class="el-icon-user-solid"></i> QQ群：
              <a>972107977</a>
            </p>
            <p>
              <i class="el-icon-user-solid"></i> 邮箱：
              <a>574427343@qq.com</a>
            </p>
            <p>
              <el-row :gutter="5">
                <el-col :xs="12" :sm="12" :md="8" :lg="8">
                  <el-button type="primary" icon="Cloudy" plain style="width: 100%;"
                    @click="goTarget('https://pcn3ojogrp79.feishu.cn/wiki/VO07wArXCibz5Jkvzejcn8CTn8f')">飞书手册</el-button>
                </el-col>
                <el-col :xs="12" :sm="12" :md="8" :lg="8">
                  <el-button type="success" icon="Cloudy" plain style="width: 100%;"
                    @click="goTarget('https://gitee.com/tylerzhou/Antflow/blob/master/antflow-vue/public/docs/%E7%B3%BB%E7%BB%9F%E6%93%8D%E4%BD%9C%E6%89%8B%E5%86%8C-%E4%BD%8E%E4%BB%A3%E7%A0%81%E7%89%88%E6%9C%AC.md')">操作手册【低代码表单】</el-button>
                </el-col>
                <el-col :xs="12" :sm="12" :md="8" :lg="8">
                  <el-button type="success" icon="Cloudy" plain style="width: 100%;"
                    @click="goTarget('https://gitee.com/tylerzhou/Antflow/blob/master/antflow-vue/public/docs/%E7%B3%BB%E7%BB%9F%E6%93%8D%E4%BD%9C%E6%89%8B%E5%86%8C-%E8%87%AA%E5%AE%9A%E4%B9%89%E8%A1%A8%E5%8D%95.md')">操作手册【自定义表单】</el-button>
                </el-col>
                <el-col :xs="12" :sm="12" :md="8" :lg="8">
                  <el-button type="success" icon="Cloudy" plain style="width: 100%;"
                    @click="goTarget('https://gitee.com/tylerzhou/Antflow/tree/master/doc')">日常开发文档</el-button>
                </el-col>
              </el-row>
            </p>
          </div>
        </el-card>
      </el-col>
    </el-row>
    <el-divider />
    <el-row :gutter="20">
      <el-col :xs="24" :sm="24" :md="12" :lg="12">
        <el-card class="update-log">
          <template v-slot:header>
            <div class="clearfix">
              <span>更新日志</span>
            </div>
          </template>
          <el-collapse accordion>
            <el-collapse-item title="v1.2.0 - 2025-06-27">
              <ol>
                <li>加签/减签/变更处理人/作废/撤回等功能上线</li>
              </ol>
            </el-collapse-item>
            <el-collapse-item title="v1.0.0 - 2025-06-27">
              <ol>
                <li>正式发布1.0版本，gitee获的star突破1000</li>
              </ol>
            </el-collapse-item>
            <el-collapse-item title="v0.9.0 - 2025-06-27">
              <ol>
                <li>流程通知模块对接</li>
              </ol>
            </el-collapse-item>
            <el-collapse-item title="v0.8.0 - 2025-05-01">
              <ol>
                <li>流程设计条件节点实现条件分组</li>
              </ol>
            </el-collapse-item>
            <el-collapse-item title="v0.7.0 - 2025-03-05">
              <ol>
                <li>实现发起人自选审批人</li>
              </ol>
            </el-collapse-item>
            <el-collapse-item title="v0.6.0 - 2025-02-15">
              <ol>
                <li>添加并行审批功能</li>
                <li>代码深度优化</li>
                <li>关键代码添加注释</li>
              </ol>
            </el-collapse-item>
            <el-collapse-item title="v0.5.0 - 2025-01-11">
              <ol>
                <li>低代码表单优化</li>
              </ol>
            </el-collapse-item>
            <el-collapse-item title="v0.4.0 - 2024-12-14">
              <ol>
                <li>低代码表单接入</li>
              </ol>
            </el-collapse-item>
            <el-collapse-item title="v0.3.1 - 2024-10-17">
              <ol>
                <li>三方业务表单接入</li>
                <li>三方业务方注册，应用注册</li>
                <li>三方业务流程设计</li>
              </ol>
            </el-collapse-item>
            <el-collapse-item title="v0.3.0 - 2024-08-10">
              <ol>
                <li>其他细节优化</li>
              </ol>
            </el-collapse-item>
            <el-collapse-item title="v0.2.0 - 2024-08-08">
              <ol>
                <li>Mock Data</li>
                <li>其他细节优化</li>
              </ol>
            </el-collapse-item>
            <el-collapse-item title="v0.1.0 - 2024-08-01">
              <ol>
                <li>防RuoYi-Vue版前端正式发布</li>
              </ol>
            </el-collapse-item>
          </el-collapse>
        </el-card>
      </el-col>
      <el-col :xs="24" :sm="24" :md="12" :lg="12" style="display: none;">
        <el-card shadow="always">
          <template v-slot:header>
            <div class="clearfix">
              <span>柱状图</span>
            </div>
          </template>
          <div class="body">
            <BarLabelRotation style="height: 580px" />
          </div>
        </el-card>
      </el-col>
    </el-row>
  </div>
</template>

<script setup name="Index">
import shortcuts from "@/components/Dashboard/Shortcuts"
import useSettingsStore from '@/store/modules/settings'
import BarLabelRotation from '@/components/ECharts/eZhuZhuangTu/BarLabelRotation.vue'
const settingsStore = useSettingsStore()
const version = settingsStore.version;
console.log("VITE_APP_ENV=======", import.meta.env.VITE_APP_ENV);
console.log("VITE_APP_BASE_API=======", import.meta.env.VITE_APP_BASE_API);
console.log("BASE_URL=======", import.meta.env.BASE_URL);
console.log("VITE_HOME_PATH=======", import.meta.env.VITE_HOME_PATH);

const envName = import.meta.env.VITE_HOME_PATH || 'admin';
const readmeForDotnet = ".net首个钉钉工作流UI风格的流程引擎！支持数十种数据库，开箱即用。同时也是一款纯血版工作流引擎，仅依赖了asp.net 9.0和freesql，natasha等几款优秀开源项目。致力解决传统工作流流程图必须由专业程序员绘制、学习曲线陡峭、上手难、排查问题难、维护成本高等问题。如果喜欢请给颗star"
const readmeForJava = "AntFlow是一款采用Springboot+Mybatis+activiti+vue3等主流前后端技术开发的仿钉钉workflow工作流引擎，结合中国式办公特点深度定制，可以作为钉钉工作流的开源替代。致力解决传统activiti/flowable流程图必须由专业程序员绘制，学习曲线陡峭，上手难、排查问题难、维护成本高等问题,是企业审批流&saas化工作流部署不二选择!如果喜欢请给颗星星！"
const readmeText = envName.indexOf('dotnet') > 0 ? readmeForDotnet : readmeForJava;

function goTarget(url) {
  window.open(url, '__blank')
}
</script>

<style scoped lang="scss">
.el-row {
  padding-top: 10px;
  padding-right: 10px;
  padding-left: 10px;
}

.el-col {
  padding-bottom: 10px;
}

.home {
  blockquote {
    padding: 10px 20px;
    margin: 0 0 20px;
    font-size: 17.5px;
    border-left: 5px solid #eee;
  }

  hr {
    margin-top: 20px;
    margin-bottom: 20px;
    border: 0;
    border-top: 1px solid #eee;
  }

  .col-item {
    margin-bottom: 20px;
  }

  .item {
    margin-top: 10px;
    margin-right: 10px;
  }

  ul {
    padding: 0;
    margin: 0;
  }

  font-family: "open sans",
  "Helvetica Neue",
  Helvetica,
  Arial,
  sans-serif;
  font-size: 13px;
  color: #676a6c;
  overflow-x: hidden;

  ul {
    list-style-type: none;
  }

  h4 {
    margin-top: 0px;
  }

  h2 {
    margin-top: 10px;
    font-size: 26px;
    font-weight: 100;
  }

  p {
    margin-top: 10px;

    b {
      font-weight: 700;
    }
  }

  .update-log {
    ol {
      display: block;
      list-style-type: decimal;
      margin-block-start: 1em;
      margin-block-end: 1em;
      margin-inline-start: 0;
      margin-inline-end: 0;
      padding-inline-start: 40px;
    }
  }
}
</style>
