export function uploadImage(loader)
{
    return {
        upload :() => {
            return new Promise(async (resolve, reject) => {
                loader.file.then(async (file) => {
                    try{
                   if(!file) return;
                  const data = new FormData();
                  data.append("files" , file); 
                //  formData.append('code', "NEWS")
                  axios.post(`${process.env.VUE_APP_API_URL}File/upload`,data).then((response) => {
                    console.log("response.data.code ", response.data.code);
                    if (response.data != null && response.data.code == 0)
                        {
                            resolve({
                                default: response.data.data, // Trả về URL trực tiếp từ API
                            });
                        }
                  }).catch((error) => {
                    // Handle error here
                    console.error('Lỗi khi upload file:', error);
                  });
                    }catch (error){
                        reject(error);
                    }   
                })
              
            })
        }
    }
}