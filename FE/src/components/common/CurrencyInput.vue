<template>
  <input
    :value="displayValue"
    type="text"
    inputmode="numeric"
    autocomplete="off"
    @input="handleInput"
    @blur="$emit('blur', $event)"
  >
</template>

<script setup>
import { computed } from 'vue';
import { formatVndNumber, parseVnd } from '@/utils/currency';

const props = defineProps({
  modelValue: { type: [Number, String], default: null }
});
const emit = defineEmits(['update:modelValue', 'blur']);
const displayValue = computed(() => formatVndNumber(props.modelValue));

function handleInput(event) {
  const value = parseVnd(event.target.value);
  event.target.value = formatVndNumber(value);
  emit('update:modelValue', value);
}
</script>
