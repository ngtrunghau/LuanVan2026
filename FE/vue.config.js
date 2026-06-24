const path = require('path');
const { defineConfig } = require('@vue/cli-service');
const CopyWebpackPlugin = require('copy-webpack-plugin');
const url = process.env.VUE_APP_URL || process.env.VUE_WEB_URL;



module.exports = defineConfig({
  transpileDependencies: ['vuetify'],
  lintOnSave: false,

  publicPath: "/",
  css: {
    extract: false,
  },
  chainWebpack: (config) => {
    config.resolve.alias
      .set('swiper$', 'swiper/js/swiper.js')
      .end();
  },

  configureWebpack: {
    module: {
      rules: [
        {
          test: /\.ico$/,
          loader: 'file-loader',
          options: {
            name: '[name].[ext]'
          }
        },

      ]
    },
    plugins: [
      new CopyWebpackPlugin({
        patterns: [
          {
            from: path.resolve(__dirname, 'src/assets/css'),
            to: path.resolve(__dirname, 'dist/css'),
          },
          {
            from: path.resolve(__dirname, 'src/assets/admin/css'),
            to: path.resolve(__dirname, 'dist/admin/css'),
          },
        ],
      }),
    ],
  },
  pluginOptions: {
    sitemap: {
      urls: [url].filter(Boolean)
    }
  }

});
