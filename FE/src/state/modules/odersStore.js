import {apiClient} from "@/state/modules/apiClient";
const controller = "Oders";

export const state = {
    reloadAuthUser: false,
}

export const mutations = {
    SET_RELOADAUTHUSER(state, newValue) {
        state.reloadAuthUser = newValue
    }
}
export const actions = {
    async getAll({commit}) {
        return apiClient.get(controller + "/get-all-core");
    },
    async getPagingParams({commit}, params) {
        return apiClient.post(controller +"/get-paging-params-core", params);
    },
    async getPagingParamsStatus2({commit}, params) {
        return apiClient.post(controller +"/get-paging-params-core-status2", params);
    },
    async getPagingParamsStatus3({commit}, params) {
        return apiClient.post(controller +"/get-paging-params-core-status3", params);
    },
    async getPagingParamsStatus3Customer({commit}, params) {
        return apiClient.post("Customer/" + controller +"/get-paging-params-core-status3", params);
    },
    async create({commit}, values) {
        return apiClient.post(controller +"/create", values);
    },
    async createCustomer({commit}, values) {
        return apiClient.post("Customer/" + controller +"/create", values);
    },
    async update({commit, dispatch}, values) {
        return apiClient.post(controller +"/update", values);
    },
    async delete({commit}, id) {
        return await apiClient.post(controller +"/delete" , id);
    },
    async getById({commit}, id) {
        return apiClient.post(controller +"/get-by-id-core", id);
    },
    async getByIdCongDan({commit}, id) {
        return apiClient.get(controller +"/get-by-id-cong-dan/" + id);
    },
};
