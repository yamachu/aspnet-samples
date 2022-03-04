import { defineConfig } from "vite";
import vue from "@vitejs/plugin-vue";

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [vue()],
  server: {
    proxy: {
      "^/(?!$|src/|node_modules/|@|assets/)": {
        target: "https://localhost:5001",
        secure: false,
      },
    },
  },
});
