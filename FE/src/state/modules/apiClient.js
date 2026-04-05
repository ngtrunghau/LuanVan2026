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
    return Promise.resolve({error});
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
                resultString: e.toString(),
                resultCode: "20"
            };
        }
    }
}

export const apiClient = new ApiClient();
