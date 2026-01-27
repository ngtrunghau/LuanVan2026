
import { toast } from "vue3-toastify";
import "vue3-toastify/dist/index.css";

const initialNotify = { message: null, code: null };

export const actions = {
    // eslint-disable-next-line no-unused-vars
  async addNotify({ commit }, data = initialNotify) {
      switch (data.code)
      {
          case  0 :
              toast(data.message, {
                  "theme": "colored",
                  "type": "success",
                  "autoClose": 2000,
                  "hideProgressBar": false,
                  "dangerouslyHTMLString": true
              })
              break;
          case 23 :
              toast(data.message, {
              "theme": "colored",
              "type": "warning",
              "autoClose": 2000,
              "hideProgressBar": false,
              "dangerouslyHTMLString": true
          })
              break;
          default:
              toast(data.message, {
                          "theme": "colored",
                          "type": "error",
                          "autoClose": 2000,
                          "hideProgressBar": false,
                          "dangerouslyHTMLString": true
                      })
      }
  },
};
