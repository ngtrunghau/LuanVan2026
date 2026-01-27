import {apiClient} from "@/state/modules/apiClient";
const controller = "District";

export const state = {
    reloadAuthUser: false,
}

export const mutations = {
    SET_RELOADAUTHUSER(state, newValue) {
        state.reloadAuthUser = newValue
    }
}
export const actions = {
    async getAll({commit}, params) {
        return apiClient.post(controller + "/get-all-core", params);
    },
};
