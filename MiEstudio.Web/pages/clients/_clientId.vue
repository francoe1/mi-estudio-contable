<script>
import { ClientStore } from './_clientId/ClientStore';

export default {
  name: 'ClientPage',
  computed: {
    store() {
      return ClientStore();
    },
  },
  async mounted() {
    await this.store.downloadClient(this.$route.params.clientId);
  },
  render() {
    if (this.store.client == null) return <b-loading active={true} />;
    return (
      <section>
        <Nuxt />
      </section>
    );
  },
};
</script>
