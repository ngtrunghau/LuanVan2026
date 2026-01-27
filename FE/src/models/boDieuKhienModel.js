const getJson = (item) => {
    return {
      id: item.id,
      name: item.name,
      key: item.key,
      isDeleted : item.isDeleted,
    };
  };
  const sendJson = (item) => {
    return {
      id: item.id,
      name: item.name,
      key: item.key,
      isDeleted : item.isDeleted,
    };
  };

  const baseJson = () => {
    return {
      name: null,
      key: "01",
      isDeleted : false,
    };
  };

  export const boDieuKhienModel = {
    sendJson , getJson, baseJson,
  };
