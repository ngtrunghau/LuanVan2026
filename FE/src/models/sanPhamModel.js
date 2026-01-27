const getJson = (item) => {
    return {
      id: item.id,
      name: item.name,
      descriptions : item.descriptions,
      price : item.price,
      stockQuantity : item.stockQuantity,
      color : item.color,
      isDeleted: item.isDeleted,
      categoriesId: item.categoriesId,
      imageUrl: item.imageUrl
    };
  };
  const sendJson = (item) => {
    return {
      id: item.id,
      name: item.name,
      descriptions : item.descriptions,
      price : item.price,
      stockQuantity : item.stockQuantity,
      color : item.color,
      isDeleted: item.isDeleted,
      categoriesId: item.categoriesId,
      imageUrl: item.imageUrl,
    };
  };

  const baseJson = () => {
    return {
      name: null,
      descriptions : null,
      price : 0,
      stockQuantity : 1,
      color : null,
      isDeleted: false,
      categoriesId: null,
      imageUrl: null
    };
  };

  export const sanPhamModel = {
    sendJson , getJson, baseJson,
  };
