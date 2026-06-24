import { apiClient } from "@/state/modules/apiClient";

const controller = "Promotion";

export const actions = {
  async getPaging({ commit }, values) {
    return apiClient.post(`${controller}/paging`, values);
  },
  async create({ commit }, values) {
    return apiClient.post(`${controller}/create`, values);
  },
  async update({ commit }, values) {
    return apiClient.post(`${controller}/update`, values);
  },
  async setActive({ commit }, values) {
    return apiClient.post(`${controller}/set-active`, values);
  },
};
