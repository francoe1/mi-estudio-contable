<script>
import { Currency } from '~/components/Currency.vue';

const confirmWaiter = dialog => {
  return new Promise(resolve => {
    dialog.onConfirm = () => resolve(true);
    dialog.onCancel = () => resolve(false);
  });
};

export default {
  name: 'Client',
  data() {
    return {
      applyExpenseActive: false,
    };
  },
  methods: {
    async applyExpense() {
      if (
        !(await confirmWaiter(
          this.$buefy.dialog.confirm({
            title: 'Liquidar abonos',
            message: 'Esta acción creara un movimiento "credito" con el valor del abono de cada cliente',
            confirmText: 'Continuar',
            cancelText: 'Cancelar',
          }),
        ))
      )
        return;

      this.applyExpenseActive = true;

      try {
        const result = (
          await this.$axios.post(`Client/ApplyExpense`, null, {
            timeout: 1000 * 30,
          })
        ).data;
        await this.$refs['List'].changePage(0);

        this.$buefy.toast.open({ message: `Se liquidaron ${result.update} de ${result.requireUpdate}` });
      } catch {
      } finally {
        this.applyExpenseActive = false;
      }
    },
  },
  render() {
    return (
      <div>
        <b-modal active={this.applyExpenseActive} closable={false}>
          <div class='container box'>
            <p>Liquidando abonos</p>
          </div>
        </b-modal>
        <div class='container mx-auto my-4 columns'>
          <div class='column box'>
            <List ref='List' />
          </div>
          <div class='column is-3 pt-0' style='min-width:300px'>
            <a class='box mb-2 is-clickable is-flex is-align-items-center' href='/clients/create'>
              <b-icon icon='account-plus' class='mr-2' size='is-small' />
              Nuevo Cliente
              <b-icon icon='arrow-right' size='is-small' class='ml-auto' />
            </a>

            <a class='box my-2 is-clickable is-flex is-align-items-center' href={`${this.$axios.defaults.baseURL}Client/Report`}>
              <b-icon icon='file-excel' class='mr-2' size='is-small' />
              Reporte completo
              <b-icon icon='download' size='is-small' class='ml-auto' />
            </a>

            <a class='box my-2 is-clickable is-flex is-align-items-center' onClick={this.applyExpense}>
              <b-icon icon='server-security' class='mr-2' size='is-small' />
              Liquidar Abonos
              <b-icon icon='plus' size='is-small' class='ml-auto' />
            </a>
          </div>
        </div>
      </div>
    );
  },
};

const List = {
  data() {
    return {
      source: { data: [] },
      filter: {
        Search: '',
        Page: 0,
        Sort: 'Name',
        PerPage: 10,
      },
    };
  },
  async mounted() {
    await this.requestSource();
  },
  watch: {
    async 'filter.Search'() {
      this.filter.Page = 0;
      await this.requestSource();
    },

    async 'filter.Sort'() {
      this.filter.Page = 0;
      await this.requestSource();
    },
    async 'filter.PerPage'() {
      this.filter.Page = 0;
      await this.requestSource();
    },
  },
  methods: {
    async requestSource() {
      this.source = (
        await this.$axios.get(`Client?Search=${this.filter.Search}&Page=${this.filter.Page}&Sort=${this.filter.Sort}&PerPage=${this.filter.PerPage}`)
      ).data;
    },

    async changePage(page) {
      this.filter.Page = page;
      await this.requestSource();
    },
  },
  render() {
    return (
      <div>
        <b-field message={`Se encontraron ${this.source.total} clientes`}>
          <b-input icon-right='magnify' placeholder='Buscar cliente' lazy={true} v-model={this.filter.Search} />
        </b-field>

        <div class='is-flex is-justify-content-end'>
          <span class='mr-2 is-size-7'>Ordernar</span>
          <select style='outline:none; border:none;' v-model={this.filter.Sort}>
            <option value='Name'>Nombre</option>
            <option value='Type'>Tipo</option>
            <option value='Balance'>Balance</option>
            <option value='Expense'>Abono</option>
            <option value='City'>Ciudad</option>
          </select>
          <span class='mx-2 is-size-7'>Mostrar</span>
          <select style='outline:none; border:none;' v-model={this.filter.PerPage}>
            <option value='5'>5</option>
            <option value='10'>10</option>
            <option value='30'>30</option>
            <option value='50'>50</option>
            <option value='100'>100</option>
          </select>
        </div>

        <div>
          {this.source.data.map(client => (
            <a
              key={client.id}
              onClick={() => this.$router.push(`/clients/${client.id}`)}
              class='is-flex is-clickable my-2 p-2 is-hoverable has-text-black-bis'
              style='border-bottom:1px solid rgb(230,230,230)'
            >
              <div>
                <p class='m-0'>{client.name}</p>
                <p class='is-size-7 has-text-grey-light'>
                  {client.type} | {client.cuit}
                </p>
              </div>
              <div class='ml-auto has-text-right'>
                <p class='m-0'>
                  <Currency value={client.balance} />
                </p>
                <p class='is-size-7 has-text-grey-light'>
                  abono <Currency value={client.expense} />
                </p>
              </div>

              <div class='is-flex is-justify-content-center is-align-items-center ml-4 mr-0'>
                <b-icon icon='arrow-right' />
              </div>
            </a>
          ))}
        </div>

        <b-pagination size='is-small' v-model={this.source.page} perPage={this.source.perPage} total={this.source.total} onChange={this.changePage} />
      </div>
    );
  },
};
</script>

<style>
div .is-hoverable:hover {
  transition: all 300ms ease-in-out;
  background: rgb(235, 235, 235);
}
</style>
