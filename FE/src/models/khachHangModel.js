const getJson = (item) => {
    return {
      id: item.id,
      fullName: item.fullName,
      phone : item.phone,
      email : item.email,
      userName : item.userName,
      password : item.password,
      isDeleted: item.isDeleted,
    };
  };
  const sendJson = (item) => {
    return {
      id: item.id,
      fullName: item.fullName,
      phone : item.phone,
      email : item.email,
      userName : item.userName,
      password : item.password,
      isDeleted: item.isDeleted,
    };
  };

  const baseJson = () => {
    return {
      fullName: null,
      phone : null,
      email : null,
      userName : null,
      password : null,
      isDeleted: false,
    };
  };

  export const khachHangModel = {
    sendJson , getJson, baseJson,
  };
