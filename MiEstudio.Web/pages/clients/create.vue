<script>
export default {
  name: 'ClientPanel',
  data() {
    return {
      client: {},
    };
  },
  methods: {
    async create() {
      this.client = (
        await this.$axios.post(`Client`, {
          ...this.client,
        })
      ).data;

      this.$router.push(`/clients/${this.client.id}`);
    },
  },
  render() {
    return (
      <div class='container mt-4'>
        <div class='p-0 box'>
          <span class='label px-4 py-3 m-0'>Información del cliente</span>
          <hr class='m-0 p-0' />
          <div class='p-4'>
            <b-field label='Nombre'>
              <b-input v-model={this.client.name} />
            </b-field>
            <b-field label='CUIT'>
              <b-select v-model={this.client.type}>
                <option value='ResponsableInscripto'>ResponsableInscripto</option>
                <option value='ResponsableNoInscripto'>ResponsableNoInscripto</option>
                <option value='ConsumidorFinal'>ConsumidorFinal</option>
                <option value='ExentoDeIVA'>ExentoDeIVA</option>
                <option value='NoResponsable'>NoResponsable</option>
              </b-select>
              <b-input v-model={this.client.cuit} expanded />
            </b-field>
            <b-field label='Ciudad/Domicilio'>
              <b-input v-model={this.client.city} />
              <b-input v-model={this.client.address} />
            </b-field>

            <b-field label='Nota'>
              <b-input type='textarea' v-model={this.client.note} />
            </b-field>

            <div class='is-flex is-justify-content-end'>
              <b-button label='Crear' type='is-success' class='ml-2' onClick={this.create} />
            </div>
          </div>
        </div>
      </div>
    );
  },
};
</script>
