<template>
    <ckeditor style="width: 1000px;" :editor="editor" v-model="value" :config="editorConfig">

    </ckeditor>
</template>

<script>
import ClassicEditor from "@/components/ckeditor5";
import {defineComponent ,ref } from '@vue/runtime-core'
import { uploadImage } from "@/utils/functions/uploadImage";
import { watch } from "vue";



export default defineComponent ({

  props :{
     editorData : {
      type : String ,
      requied : true,
     },
  },

  setup( {editorData}) {
    function uploader(editor){
      editor.plugins.get( 'FileRepository' ).createUploadAdapter = (loader) =>{
        return uploadImage(loader);
      };
    }


    const value = ref(editorData);
    watch(value , (value) => {
  //    console.log("LOG WATCH  : ", value);
    });

    return {
      editor: ClassicEditor,
      value,
      editorConfig: {
        extraPlugins : [uploader],
      },
    };
  }
})
</script>
