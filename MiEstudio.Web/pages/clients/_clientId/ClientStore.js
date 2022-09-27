import { defineStore } from 'pinia';
import { AxiosInstance } from '/plugins/axios';

export const ClientStore = defineStore('ClientStore', {
  state() {
    return {
      client: null,
      movements: { data: [] },
    };
  },
  actions: {
    async downloadClient(clientId) {
      this.client = (await AxiosInstance.get(`Client/${clientId}`)).data;
      await this.downloadMovement(clientId);
    },

    async downloadMovement(clientId) {
      this.movements = (await AxiosInstance.get(`Client/${clientId}/Movement?PerPage=10`)).data;
    },

    async createMovement(clientId, data) {
      await AxiosInstance.post(`Client/${clientId}/Movement`, { ...data, expireDate: new Date() });
      await this.downloadClient(clientId);
    },

    async applyExpense(clientId, data) {
      await AxiosInstance.post(`Client/${clientId}/Expense`, { ...data });
      await this.downloadClient(clientId);
    },
  },
});
