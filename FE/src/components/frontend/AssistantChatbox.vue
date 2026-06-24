<template>
  <div class="assistant-chatbox" v-if="isVisibleOnRoute">
    <button v-if="!isOpen" class="assistant-toggle" @click="toggleOpen">
      <i class="fa-solid fa-comments"></i>
      <span>Trợ lý mua sắm</span>
    </button>

    <div v-else class="assistant-panel shadow">
      <div class="assistant-header">
        <div>
          <h6 class="mb-0">Trợ lý ShopNTH</h6>
          <small>Tư vấn theo sản phẩm thực tế của cửa hàng</small>
        </div>
        <div class="header-actions">
          <button class="close-btn" type="button" aria-label="Xóa cuộc trò chuyện" @click="resetChat">
            <i class="fa-solid fa-rotate-right"></i>
          </button>
          <button class="close-btn" type="button" aria-label="Đóng trợ lý" @click="toggleOpen">
          <i class="fa-solid fa-xmark"></i>
          </button>
        </div>
      </div>

      <div class="assistant-body" ref="messagesContainer">
        <div
          v-for="(message, idx) in messages"
          :key="idx"
          :class="['msg-row', message.role === 'user' ? 'user' : 'bot']"
        >
          <div class="msg-bubble">
            <p class="mb-0">{{ message.text }}</p>

            <div v-if="message.suggestions && message.suggestions.length" class="suggestions mt-2">
              <router-link
                v-for="item in message.suggestions"
                :key="item.productId"
                :to="`/san-pham-chi-tiet/${item.productId}`"
                class="suggestion-item"
                @click="isOpen = false"
              >
                <img :src="item.imageUrl || fallbackImage" alt="Product" />
                <div class="suggestion-info">
                  <strong>{{ item.name }}</strong>
                  <span>{{ formatCurrency(item.price) }}</span>
                  <em>
                    {{ item.categoryName }}
                    <template v-if="item.color"> · {{ item.color }}</template>
                    · {{ item.stockQuantity > 0 ? `Còn ${item.stockQuantity}` : "Hết hàng" }}
                  </em>
                  <small>{{ item.reason }}</small>
                </div>
              </router-link>
            </div>

            <div
              v-if="message.quickReplies && message.quickReplies.length"
              class="quick-replies mt-2"
            >
              <button
                v-for="(reply, qIdx) in message.quickReplies"
                :key="qIdx"
                class="reply-chip"
                @click="useQuickReply(reply)"
              >
                {{ reply }}
              </button>
            </div>
          </div>
        </div>

        <div v-if="isLoading" class="msg-row bot">
          <div class="msg-bubble typing">
            <span></span><span></span><span></span>
          </div>
        </div>
      </div>

      <form class="assistant-footer" @submit.prevent="sendMessage">
        <input
          v-model="draftMessage"
          type="text"
          maxlength="1000"
          aria-label="Nội dung cần tư vấn"
          placeholder="Ví dụ: Vợt Yonex còn hàng dưới 2 triệu"
        />
        <button
          type="submit"
          aria-label="Gửi tin nhắn"
          :disabled="isLoading || !draftMessage.trim()"
        >
          <i class="fa-solid fa-paper-plane"></i>
        </button>
      </form>
    </div>
  </div>
</template>

<script setup>
import { computed, getCurrentInstance, reactive, toRefs } from "vue";
defineOptions({
  name: "AssistantChatbox"
});
const {
  proxy
} = getCurrentInstance();
const state = reactive({
  isOpen: false,
  isLoading: false,
  draftMessage: "",
  fallbackImage: require("@/assets/img/caulong/logo/logoNTH_removeBackground.png"),
  messages: []
});
const {
  isOpen,
  isLoading,
  draftMessage,
  fallbackImage,
  messages
} = toRefs(state);
function toggleOpen() {
  state.isOpen = !state.isOpen;
  proxy.$nextTick(scrollToBottom);
}
function useQuickReply(reply) {
  state.draftMessage = reply;
  sendMessage();
}
function initialMessage() {
  return {
    role: "bot",
    text: "Chào bạn! Mình có thể tìm sản phẩm theo tên, danh mục, màu sắc, ngân sách, tồn kho hoặc mức độ bán chạy. Bạn đang cần gì?",
    quickReplies: ["Cửa hàng đang bán những gì?", "Sản phẩm bán chạy", "Sản phẩm còn hàng dưới 2 triệu"]
  };
}
function restoreChat() {
  try {
    const saved = JSON.parse(sessionStorage.getItem("shopnth-assistant-chat") || "[]");
    state.messages = Array.isArray(saved) && saved.length ? saved : [initialMessage()];
  } catch (e) {
    state.messages = [initialMessage()];
  }
}
function persistChat() {
  try {
    sessionStorage.setItem("shopnth-assistant-chat", JSON.stringify(state.messages.slice(-30)));
  } catch (e) {
    // Trình duyệt có thể chặn sessionStorage; chat vẫn hoạt động trong bộ nhớ.
  }
}
function resetChat() {
  state.messages = [initialMessage()];
  state.draftMessage = "";
  persistChat();
  proxy.$nextTick(scrollToBottom);
}
function buildHistory() {
  return state.messages.slice(-12).map(message => {
    const productNames = (message.suggestions || []).map(x => x.name).join(", ");
    return {
      role: message.role === "user" ? "user" : "assistant",
      content: productNames ? `${message.text} Sản phẩm: ${productNames}` : message.text
    };
  });
}
function getLastSuggestedProductIds() {
  const lastSuggestionMessage = [...state.messages].reverse().find(message => message.suggestions && message.suggestions.length);
  return lastSuggestionMessage ? lastSuggestionMessage.suggestions.map(x => x.productId) : [];
}
async function sendMessage() {
  const content = (state.draftMessage || "").trim();
  if (!content || state.isLoading) {
    return;
  }
  const history = buildHistory();
  const lastSuggestedProductIds = getLastSuggestedProductIds();
  state.messages.push({
    role: "user",
    text: content
  });
  state.draftMessage = "";
  state.isLoading = true;
  persistChat();
  proxy.$nextTick(scrollToBottom);
  try {
    const res = await proxy.$store.dispatch("assistantStore/ask", {
      message: content,
      maxResults: 5,
      history,
      lastSuggestedProductIds
    });
    if (res && res.code === 0 && res.data) {
      state.messages.push({
        role: "bot",
        text: res.data.message || "Minh da tim duoc mot vai goi y cho ban.",
        suggestions: res.data.suggestions || [],
        quickReplies: res.data.quickReplies || []
      });
    } else {
      state.messages.push({
        role: "bot",
        text: "Minh chua phan hoi duoc luc nay, ban thu lai sau giup minh nhe."
      });
    }
  } catch (e) {
    state.messages.push({
      role: "bot",
      text: "Co loi khi ket noi tro ly. Ban vui long thu lai."
    });
  } finally {
    state.isLoading = false;
    persistChat();
    proxy.$nextTick(scrollToBottom);
  }
}
function formatCurrency(value) {
  return new Intl.NumberFormat("vi-VN", {
    style: "currency",
    currency: "VND"
  }).format(value || 0);
}
function scrollToBottom() {
  const el = proxy.$refs.messagesContainer;
  if (el) {
    el.scrollTop = el.scrollHeight;
  }
}
const isVisibleOnRoute = computed(() => {
  const path = proxy.$route.path || "";
  return !path.startsWith("/quan-tri");
});
restoreChat();
</script>

<style scoped>
.assistant-chatbox {
  position: fixed;
  right: 18px;
  bottom: 18px;
  z-index: 1200;
}

.assistant-toggle {
  border: 0;
  border-radius: 999px;
  background: linear-gradient(135deg, #1f7a8c, #2a9d8f);
  color: #fff;
  padding: 10px 16px;
  display: flex;
  gap: 10px;
  align-items: center;
  font-weight: 600;
  box-shadow: 0 12px 28px rgba(0, 0, 0, 0.2);
}

.assistant-panel {
  width: 360px;
  max-width: calc(100vw - 20px);
  height: 560px;
  max-height: calc(100vh - 40px);
  border-radius: 16px;
  overflow: hidden;
  background: #f6f8fa;
  display: flex;
  flex-direction: column;
  border: 1px solid #d8dee4;
}

.assistant-header {
  background: linear-gradient(135deg, #0f4c5c, #2a9d8f);
  color: #fff;
  padding: 12px 14px;
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.assistant-header small {
  opacity: 0.9;
}

.header-actions {
  display: flex;
  align-items: center;
  gap: 4px;
}

.close-btn {
  border: 0;
  background: transparent;
  color: #fff;
  font-size: 18px;
}

.assistant-body {
  flex: 1;
  overflow-y: auto;
  padding: 12px;
}

.msg-row {
  display: flex;
  margin-bottom: 10px;
}

.msg-row.user {
  justify-content: flex-end;
}

.msg-bubble {
  max-width: 88%;
  border-radius: 12px;
  padding: 10px 12px;
  background: #fff;
  border: 1px solid #e5e7eb;
}

.msg-row.user .msg-bubble {
  background: #d9f2ec;
  border-color: #9dd8ca;
}

.suggestions {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.suggestion-item {
  display: flex;
  gap: 8px;
  text-decoration: none;
  color: inherit;
  background: #ffffff;
  border: 1px solid #dfe3e8;
  border-radius: 10px;
  padding: 8px;
}

.suggestion-item img {
  width: 52px;
  height: 52px;
  object-fit: cover;
  border-radius: 8px;
  border: 1px solid #d0d7de;
}

.suggestion-info {
  display: flex;
  flex-direction: column;
  min-width: 0;
}

.suggestion-info strong {
  line-height: 1.25;
}

.suggestion-info span {
  color: #0f766e;
  font-weight: 600;
  font-size: 13px;
}

.suggestion-info small {
  color: #5b6470;
  font-size: 12px;
}

.suggestion-info em {
  color: #64748b;
  font-size: 11px;
  font-style: normal;
}

.quick-replies {
  display: flex;
  flex-wrap: wrap;
  gap: 6px;
}

.reply-chip {
  border: 1px solid #b3c5cf;
  background: #eef4f7;
  border-radius: 999px;
  font-size: 12px;
  padding: 4px 10px;
}

.assistant-footer {
  border-top: 1px solid #d0d7de;
  background: #fff;
  padding: 10px;
  display: flex;
  gap: 8px;
}

.assistant-footer input {
  flex: 1;
  border-radius: 10px;
  border: 1px solid #c8d1da;
  padding: 8px 10px;
  font-size: 14px;
}

.assistant-footer button {
  border: 0;
  border-radius: 10px;
  width: 40px;
  background: #0f766e;
  color: #fff;
}

.assistant-footer button:disabled {
  background: #94a3b8;
}

.typing {
  display: flex;
  gap: 4px;
}

.typing span {
  width: 6px;
  height: 6px;
  border-radius: 50%;
  background: #7b8794;
  animation: blink 1.2s infinite ease-in-out;
}

.typing span:nth-child(2) {
  animation-delay: 0.2s;
}

.typing span:nth-child(3) {
  animation-delay: 0.4s;
}

@keyframes blink {
  0%,
  80%,
  100% {
    transform: scale(0.8);
    opacity: 0.5;
  }
  40% {
    transform: scale(1);
    opacity: 1;
  }
}

@media (max-width: 768px) {
  .assistant-chatbox {
    right: 10px;
    bottom: 10px;
  }

  .assistant-panel {
    width: calc(100vw - 20px);
    height: 70vh;
  }
}
</style>
