import axios  from "axios";
const path = process.env.VUE_APP_API_URL;
/** 400: Bad Request */
export const CLIENT_ERROR_CODE = 400;
export const httpClient = axios.create({
    baseURL: path,
    json: true,
    headers: {
        'Content-Type' : 'application/json'
    },
    timeout: 300000
})
httpClient.interceptors.response.use((response) => {
    return response;
}, async (error) => {
    if (error?.response?.status === 401) {
        localStorage.removeItem("token");
        localStorage.removeItem("auth-user");
    }
    return Promise.reject(error);
});

class ApiClient{
    getInstance(){
        try {

            let token = localStorage.getItem("token");
            if (token) {
                httpClient.defaults.headers.common['Authorization'] = `Bearer ${token}`;
            } else {
                delete httpClient.defaults.headers.common['Authorization'];
            }
            return httpClient;
        } catch (e)
        {
            return httpClient;
        }

    }
    async get(url) {
        try {
        //    console.log("LOG GET API CLIENT  ")
            const response = await this.getInstance().get(path + url);
            return response.data;
        } catch (e) {
            return {
                success: false,
                code: CLIENT_ERROR_CODE,
                message: e.toString(),
            };
        }
    }
    async post(url, data) {
        try {
            const response = await this.getInstance().post(path + url, data);
            return response.data;
        } catch (e) {
            return {
                success: false,
                code: e?.response?.status || CLIENT_ERROR_CODE,
                message: e?.response?.data?.message || e.message || "Không thể kết nối máy chủ",
                detail: e?.response?.data
            };
        }
    }
}

export const apiClient = new ApiClient();
