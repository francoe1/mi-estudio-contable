<script>
const format = value => {
  return new Intl.NumberFormat(`es-AR`, {
    currency: `ARS`,
    style: 'currency',
  }).format(value);
};

export const Currency = {
  props: {
    value: {
      type: Number,
      default: () => 0,
    },
    editable: {
      type: Boolean,
      default: () => false,
    },
  },
  data() {
    return {
      edit: false,
    };
  },
  computed: {
    formated() {
      return format(this.value);
    },
  },
  render() {
    if (this.editable) {
      return (
        <div
          contenteditable
          style={{ outline: this.edit ? '2px solid green' : null }}
          onFocusin={() => {
            this.$refs['input'].style.display = 'block';
            this.$refs['input'].focus();
            this.edit = true;
          }}
        >
          <div class='input is-expanded is-fullwidth'>{this.formated}</div>
          <input
            ref='input'
            value={this.value}
            onKeyup={e => this.$emit('input', parseFloat(e.target.value.replace('.', ',')))}
            type='number'
            style='position: fixed; top : -100px; display:none'
            onFocusout={() => {
              this.$refs['input'].style.display = 'none';
              this.edit = false;
            }}
          />
        </div>
      );
    }
    return <span>{this.formated}</span>;
  },
};

export default Currency;
</script>
