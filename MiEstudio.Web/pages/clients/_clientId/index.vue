<script>
import Currency from '~/components/Currency.vue';
import moment from 'moment';
import { ClientStore } from './ClientStore';

export default {
  name: 'ClientPage',
  computed: {
    store() {
      return ClientStore();
    },
  },
  render() {
    return (
      <section>
        <MovementModal ref='MovementModal' />
        <ExpenseModal ref='ExpenseModal' />
        <div class='columns container mx-auto my-4'>
          <div class='column is-4'>
            <ClientInfo class='box m-0' />

            <a class='box my-4 is-clickable  is-flex is-align-items-center' onClick={() => (this.$refs['ExpenseModal'].active = true)}>
              <b-icon icon='cash-lock' class='mr-2' size='is-small' />
              Modificar Abono
              <b-icon icon='arrow-right' size='is-small' class='ml-auto' />
            </a>

            <a class='box my-4 is-clickable is-flex is-align-items-center' onClick={() => (this.$refs['MovementModal'].active = true)}>
              <b-icon icon='cash-refund' class='mr-2' size='is-small' />
              Nuevo movimiento
              <b-icon icon='arrow-right' size='is-small' class='ml-auto' />
            </a>

            <ClientBalance />

            <a
              class='box my-4 is-clickable is-flex is-align-items-center'
              href={`${this.$axios.defaults.baseURL}Client/${this.store.client.id}/Report/Movement`}
              disabled={this.store.movements.data.length == 0}
            >
              <b-icon icon='file-excel' class='mr-2' size='is-small' />
              Descargar reporte como CSV
              <b-icon icon='download' size='is-small' class='ml-auto' />
            </a>
          </div>
          <div class='column'>
            <ClientPanel class='box' />
          </div>
        </div>
      </section>
    );
  },
};

const ClientInfo = {
  name: 'ClientInfo',
  computed: {
    store() {
      return ClientStore();
    },
  },
  render() {
    return (
      <div>
        <div>
          <p class='is-size-7'>Balance de cuenta corriente.</p>
          <Currency value={this.store.client.balance} class='is-size-3 is-bold mb-0 pb-0' />
          <hr class='my-1' />
          <p class='is-size-7 mt-0 pt-0'>
            <Currency value={this.store.client.expense} /> Proximo Abono
          </p>
        </div>
      </div>
    );
  },
};

const ClientPanel = {
  name: 'ClientPanel',
  data() {
    return {
      originalData: null,
    };
  },

  async mounted() {
    await this.update();
  },
  methods: {
    async update() {
      this.store.client = (await this.$axios(`Client/${this.$route.params.clientId}`)).data;
    },

    edit() {
      this.originalData = JSON.parse(JSON.stringify(this.store.client));
    },

    cancelEdit() {
      this.store.client = this.originalData;
      this.originalData = null;
    },

    async applyEdit() {
      this.store.client = (
        await this.$axios.put(`Client/${this.$route.params.clientId}`, {
          ...this.store.client,
        })
      ).data;

      this.originalData = null;
    },
  },
  computed: {
    editMode() {
      return this.originalData != null;
    },
    store() {
      return ClientStore();
    },
  },
  render() {
    return (
      <div class='p-0'>
        <span class='label px-4 py-3'>Información del cliente</span>
        <hr class='m-0 p-0' />
        <div class='p-4'>
          <b-field label='Nombre'>
            <b-input v-model={this.store.client.name} disabled={!this.editMode} />
          </b-field>
          <b-field label='CUIT'>
            <b-select v-model={this.store.client.type} disabled={!this.editMode}>
              <option value='ResponsableInscripto'>ResponsableInscripto</option>
              <option value='ResponsableNoInscripto'>ResponsableNoInscripto</option>
              <option value='ConsumidorFinal'>ConsumidorFinal</option>
              <option value='ExentoDeIVA'>ExentoDeIVA</option>
              <option value='NoResponsable'>NoResponsable</option>
            </b-select>
            <b-input v-model={this.store.client.cuit} expanded disabled={!this.editMode} />
          </b-field>
          <b-field label='Ciudad/Domicilio'>
            <b-input v-model={this.store.client.city} disabled={!this.editMode} />
            <b-input v-model={this.store.client.address} expanded disabled={!this.editMode} />
          </b-field>

          <b-field label='Nota'>
            <b-input type='textarea' v-model={this.store.client.note} disabled={!this.editMode} />
          </b-field>

          <div class='is-flex is-justify-content-end'>
            {!this.editMode && <b-button label='Modificar' type='is-info' onClick={this.edit} />}
            {this.editMode && (
              <div>
                <b-button label='Cancelar' type='is-danger' onClick={this.cancelEdit} />
                <b-button label='Guardar' type='is-success' class='ml-2' onClick={this.applyEdit} />
              </div>
            )}
          </div>
        </div>
      </div>
    );
  },
};

const ClientBalance = {
  name: 'ClientBalance',
  computed: {
    store() {
      return ClientStore();
    },
  },
  render() {
    return (
      <div class='my-4 box container p-0'>
        <span class='label m-0 px-4 py-3'>Tu actividad</span>
        <hr class='m-0 p-0' />
        <div class='px-4'>
          {this.store.movements.data.map(movement => (
            <div key={movement.id} class='is-flex py-1 px-2' style='border-bottom:1px solid rgb(230,230,230)'>
              <div>
                {movement.concept == null && <p class='has-text-info-dark'>¡Sin concepto!</p>}
                <p class='mb-0'>{movement.concept}</p>
                <p class='is-size-7'>{moment(movement.date).calendar()}</p>
              </div>
              <div class='ml-auto has-text-right'>
                <Currency value={movement.value} class={movement.type == 'Credito' ? 'has-text-danger' : 'has-text-success'} />
                <p class='is-size-7'>
                  <Currency value={movement.balance} />
                </p>
              </div>
            </div>
          ))}
        </div>
      </div>
    );
  },
};

const MovementModal = {
  name: 'Movement',
  data() {
    return {
      form: {
        type: 'Credito',
        value: 0,
        concept: '',
      },
      active: false,
    };
  },
  watch: {
    active() {
      this.form.value = 0;
      this.form.concept = '';
    },
  },
  methods: {
    async createMovement() {
      await this.store.createMovement(this.store.client.id, this.form);
      this.$emit('accept');
      this.active = false;
    },
  },
  mounted() {
    if (this.store.client.balance < 0) {
      this.form.type = 'Debito';
      this.form.value = Math.abs(this.store.client.balance);
    }
  },
  computed: {
    store() {
      return ClientStore();
    },
  },
  render() {
    return (
      <b-modal active={this.active} onClose={() => (this.active = false)}>
        <div class='card'>
          <div class='card-header'>
            <p class='p-4'>Movimiento</p>
          </div>
          <div class='card-content'>
            <b-field label='Tipo/Importe'>
              <b-select v-model={this.form.type}>
                <option value='Credito'>Credito</option>
                <option value='Debito'>Debito</option>
              </b-select>
              <Currency type='number' min={1} v-model={this.form.value} class='ml-2' editable={true} expanded />
            </b-field>
            <b-field label='Concepto'>
              <b-input type='text' v-model={this.form.concept} />
            </b-field>

            <div class='is-flex is-justify-content-end'>
              <b-button label='¡Listo!' icon-right='arrow-right' type='is-success' onClick={this.createMovement} />
            </div>
          </div>
        </div>
      </b-modal>
    );
  },
};

const ExpenseModal = {
  name: 'ExpenseModal',
  data() {
    return {
      form: {
        value: 0,
      },
      active: false,
    };
  },
  watch: {
    active() {
      this.form.value = 0;
    },
  },
  methods: {
    async accept() {
      this.store.applyExpense(this.store.client.id, { ...this.form });
      this.active = false;
    },
  },
  computed: {
    store() {
      return ClientStore();
    },
  },
  render() {
    return (
      <b-modal active={this.active} onClose={() => (this.active = false)}>
        <div class='card'>
          <div class='card-header'>
            <p class='p-4'>Abono</p>
          </div>
          <div class='card-content'>
            {this.store.client.expense > 0 && (
              <div class='notification is-danger p-2'>
                El abono actual del cliente es <Currency value={this.store.client.expense} />
              </div>
            )}
            <Currency editable={true} v-model={this.form.value} />
            <div class='is-flex is-justify-content-end mt-4'>
              <b-button label='Aplicar' icon-right='arrow-right' onClick={this.accept} />
            </div>
          </div>
        </div>
      </b-modal>
    );
  },
};
</script>
<style>
a[disabled] {
  opacity: 0.8;
  cursor: not-allowed !important;
  pointer-events: none !important;
}
</style>
