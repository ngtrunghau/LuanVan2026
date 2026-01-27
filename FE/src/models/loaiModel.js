const getJson = (item) => {
    return {
      id: item.id,
      name: item.name,
      isDeleted: item.isDeleted,
      sort: item.sort
    };
  };
  const sendJson = (item) => {
    return {
      id: item.id,
      name: item.name,
      isDeleted: item.isDeleted,
      sort: item.sort
    };
  };

  const baseJson = () => {
    return {
      name: null,
      isDeleted: false,
      sort: 0
    };
  };

  export const loaiModel = {
    sendJson , getJson, baseJson,
  };
