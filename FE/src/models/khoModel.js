const getJson = (item) => {
    return {
        id: item.id,
        productId: item.productId,
        product: item.product,
        quantityImport: item.quantityImport,
        isDeleted: item.isDeleted,
        remainQuantity: item.remainQuantity
    }
}

const sendJson = (item) => {
    return {
        id: item.id,
        productId: item.productId,
        quantityImport: item.quantityImport,
        product: item.product,
        isDeleted: item.isDeleted,
        remainQuantity: item.remainQuantity
    }
}

const baseJson = () => {
    return {
        productId: null,
        product: null,
        quantityImport: 0,
        isDeleted: false,
        remainQuantity: 0
    }
}

export const khoModel = {
    sendJson, getJson, baseJson
}
