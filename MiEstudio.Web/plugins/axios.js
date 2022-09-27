import { Axios } from 'axios';

/** @type {Axios} */
export let AxiosInstance = null;
const SetAxiosInstance = (/** @type {Axios} instance */ instance) => {
  AxiosInstance = instance;
};

export default ({ $axios }) => SetAxiosInstance($axios);
