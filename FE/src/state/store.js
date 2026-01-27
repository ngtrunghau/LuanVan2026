
import Vuex from 'vuex'

import modules from './modules'
const store = new Vuex.Store({
  modules,
  strict: process.env.NODE_ENV !== 'production',
})

export default store

