import { defineConfig, loadEnv } from "vite";
import path from "path";
import createVitePlugins from "./vite/plugins";

const baseUrl = "http://localhost:8080"; // 后端接口

// https://vitejs.dev/config/
export default defineConfig(({ mode, command }) => {
  const env = loadEnv(mode, process.cwd());
  const { VITE_APP_ENV, VITE_HOME_PATH } = env;
  return {
    // 部署生产环境和开发环境下的URL。
    // 默认情况下，vite 会假设你的应用是被部署在一个域名的根路径上
    // 例如 https://www.ruoyi.vip/。如果应用被部署在一个子路径上，你就需要用这个选项指定这个子路径。例如，如果你的应用被部署在 https://www.ruoyi.vip/admin/，则设置 baseUrl 为 /admin/。
    base: VITE_APP_ENV === "production" ? VITE_HOME_PATH : "/",
    plugins: createVitePlugins(env, command === "build"),
    resolve: {
      // https://cn.vitejs.dev/config/#resolve-alias
      alias: {
        // 设置路径
        "~": path.resolve(__dirname, "./"),
        // 设置别名
        "@": path.resolve(__dirname, "./src"),
      },
      // https://cn.vitejs.dev/config/#resolve-extensions
      extensions: [".mjs", ".js", ".ts", ".jsx", ".tsx", ".json", ".vue"],
    },
    optimizeDeps: {
      include: ["@/./lib/vForm/designer.umd.js"], //此处路径必须跟main.js中import路径完全一致！
    },
    // 打包配置
    build: {
      // https://vite.dev/config/build-options.html
      sourcemap: command === "build" ? false : "inline",
      outDir: "dist",
      assetsDir: "assets",
      chunkSizeWarningLimit: 2000,
      rollupOptions: {
        output: {
          chunkFileNames: "static/js/[name]-[hash].js",
          entryFileNames: "static/js/[name]-[hash].js",
          assetFileNames: "static/[ext]/[name]-[hash].[ext]",
          manualChunks(id) {
            // 将 vForm 库单独打包
            if (id.includes("lib/vForm")) {
              return "lib-vForm";
            }
            // 按一级目录拆分第三方依赖
            if (id.includes("node_modules")) {
              return id
                .toString()
                .split("node_modules/")[1]
                .split("/")[0]
                .toString();
            }
          },
        },
      },
      commonjsOptions: {
        include: /node_modules|lib/, //这里记得把lib目录加进来，否则生产打包会报错！！
      },
    },
    // vite 相关配置
    server: {
      port: 80,
      host: true,
      open: true,
      proxy: {
        // https://cn.vitejs.dev/config/#server-proxy
        "/dev-api": {
          target: baseUrl,
          changeOrigin: true,
          rewrite: (p) => p.replace(/^\/dev-api/, ""),
        },
        // springdoc proxy
        "^/v3/api-docs/(.*)": {
          target: baseUrl,
          changeOrigin: true,
        },
      },
    },
    css: {
      postcss: {
        plugins: [
          {
            postcssPlugin: "internal:charset-removal",
            AtRule: {
              charset: (atRule) => {
                if (atRule.name === "charset") {
                  atRule.remove();
                }
              },
            },
          },
        ],
      },
    },
  };
});
