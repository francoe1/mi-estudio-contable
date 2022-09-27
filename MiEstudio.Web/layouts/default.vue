<script>
export default {
  data() {
    return {
      items: [
        {
          title: 'Clientes',
          icon: 'Person',
          to: { name: 'clients' },
        },
      ],
    };
  },

  render() {
    return (
      <div id='layout'>
        <div class='has-background-info-light'>
          <NavBar />
        </div>
        <Nuxt class='content' />
      </div>
    );
  },
};

const NavBar = {
  render() {
    return (
      <nav class='navbar is-dark' role='navigation' aria-label='main navigation'>
        <div class='navbar-brand'>
          <a class='navbar-item' href='/'>
            MiEstudio
          </a>
        </div>

        <div class='navbar-menu'>
          <a class='navbar-item has-text-white' href='/clients'>
            Clientes
          </a>
        </div>
      </nav>
    );
  },
};
</script>
<style scoped>
#layout {
  height: 100vh;
  width: 100vw;
  display: grid;
  grid-template-columns: 1fr;
  grid-template-rows: 45px 1fr;
}
#layout > .content {
  max-height: 100vh;
  overflow: auto;
}
</style>
