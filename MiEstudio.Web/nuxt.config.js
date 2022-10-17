export default {
  ssr: false,
  head: {
    title: 'MiEstudio',
    meta: [
      { charset: 'utf-8' },
      { name: 'viewport', content: 'width=device-width, initial-scale=1' },
      { hid: 'description', name: 'description', content: '' },
      { name: 'format-detection', content: 'telephone=no' },
    ],
    link: [{ rel: 'icon', type: 'image/x-icon', href: '/favicon.ico' }],
  },

  css: ['./assets/default.css'],
  plugins: ['./plugins/axios.js'],
  components: false,
  modules: ['nuxt-buefy', '@nuxtjs/pwa', '@nuxtjs/axios'],
  buildModules: ['@nuxtjs/composition-api/module', '@pinia/nuxt'],
  axios: {
    //baseURL: 'https://localhost:7275/api/',
    baseURL: '/api/',
  },
  pwa: {
    manifest: {
      lang: 'es',
    },
  },
  build: {},
};
