const getJson = (item) => {
    return {
      id: item.id,
      totalAmount: item.totalAmount,
      provinceId : item.provinceId,
      districtId : item.districtId,
      townId : item.townId,
      customerId : item.customerId,
      isDeleted: item.isDeleted,
    };
  };
  const sendJson = (item) => {
    return {
      id: item.id,
      totalAmount: item.totalAmount,
      provinceId : item.provinceId,
      districtId : item.districtId,
      townId : item.townId,
      customerId : item.customerId,
      isDeleted: item.isDeleted,
    };
  };

  const baseJson = () => {
    return {
      totalAmount: null,
      provinceId : null,
      districtId : null,
      townId : null,
      customerId : null,
      isDeleted: false,
    };
  };

  export const odersModel = {
    sendJson , getJson, baseJson,
  };
