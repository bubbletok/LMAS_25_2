from agent import Agent
from agent_chat import AgentChat
from agent_retriever import AgentRetriever
from agent_memory import AgentMemory
from agent_behavior import AgentBehavior
from agent_planner import AgentPlanner
from agent_vad import AgentVAD

from langchain_community.chat_models import ChatOllama
from langchain_community.vectorstores import Pinecone
from langchain_openai import ChatOpenAI, OpenAIEmbeddings


from langchain_community.vectorstores import FAISS
from langchain_community.docstore.in_memory import InMemoryDocstore
import faiss

import os

from fastapi.encoders import jsonable_encoder

agent_llm_model_name = "phi4-mini"
vad_agent_model_name = "phi4-mini"

vad_agent_chat_llm = ChatOllama(model = vad_agent_model_name)
vad_chat = AgentChat(llm=vad_agent_chat_llm)
vad_agent = AgentVAD(chat=vad_chat)

# API keys & Settings
# os.environ["PINECONE_API_KEY"] = "YOUR_API_KEY"
# os.environ["PINECONE_INDEX_NAME"] = "YOUR_INDEX_NAME"   
os.environ["OPENAI_API_KEY"] = "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"

# OpenAI Embeddings
embeddings = OpenAIEmbeddings(model="text-embedding-3-small")
# FAISS Index
index = faiss.IndexFlatL2(len(embeddings.embed_query("hello world")))
# FAISS Vector Store
vector_store = FAISS(
    embedding_function=embeddings,
    index=index,
    docstore=InMemoryDocstore(),
    index_to_docstore_id={},
)

maru_chat_llm = ChatOllama(model=agent_llm_model_name)
maru_chat = AgentChat(llm=maru_chat_llm)
maru_memory = AgentMemory(chat=maru_chat)
maru_retriever = AgentRetriever(vectorstore=vector_store)
maru_behavior = AgentBehavior(chat=maru_chat)
maru_planner = AgentPlanner(chat=maru_chat)

maru_memory.add_memory(
    memory_content="My name is Maru, I am 5 years old",
    emotion="Neutral",
    time=0.0
)

# Memory Test
# maru_memory.update_memory("I am a cat", 0.5)

# For Test
maru = Agent(
    name="maru",
    age=5,
    chat=maru_chat,
    memory=maru_memory,
    retriever=maru_retriever,
    behavior=maru_behavior,
    planner=maru_planner,
    vad=vad_agent,
)

print(f"Maru: {maru.json()}")

# ethan_chat_llm = ChatOllama(model=agent_llm_model_name)
# ethan_chat = AgentChat(llm=ethan_chat_llm)
# ethan_memory = AgentMemory(chat=ethan_chat)
# ethan_retriever = AgentRetriever(vectorstore=vector_store)
# ethan_behavior = AgentBehavior(chat=ethan_chat)
# ethan_planner = AgentPlanner(chat=ethan_chat)

# ethan_memory.add_memory(
#     memory_content="My name is Ethan, I am 25 years old",
#     emotion="Neutral",
#     time=0.0
# )

# ethan = Agent(
#     name="ethan",
#     age=25,
#     chat=ethan_chat,
#     memory=ethan_memory,
#     retriever=ethan_retriever,
#     behavior=ethan_behavior,
#     planner=ethan_planner,
#     vad=vad_agent,
# )

# anya_chat_llm = ChatOllama(model=agent_llm_model_name)
# anya_chat = AgentChat(llm=anya_chat_llm)
# anya_memory = AgentMemory(chat=anya_chat)
# anya_retriever = AgentRetriever(vectorstore=vector_store)
# anya_behavior = AgentBehavior(chat=anya_chat)
# anya_planner = AgentPlanner(chat=anya_chat)

# anya_memory.add_memory(
#     memory_content="My name is Anya, I am 18 years old",
#     emotion="Neutral",
#     time=0.0
# )

# anya = Agent(
#     name="anya",
#     age=18,
#     chat=anya_chat,
#     memory=anya_memory,
#     retriever=anya_retriever,
#     behavior=anya_behavior,
#     planner=anya_planner,
#     vad=vad_agent,
# )

# mike_chat_llm = ChatOllama(model=agent_llm_model_name)
# mike_chat = AgentChat(llm=mike_chat_llm)
# mike_memory = AgentMemory(chat=mike_chat)
# mike_retriever = AgentRetriever(vectorstore=vector_store)
# mike_behavior = AgentBehavior(chat=mike_chat)
# mike_planner = AgentPlanner(chat=mike_chat)
# mike_memory.add_memory(
#     memory_content="My name is Mike, I am 35 years old",
#     emotion="Neutral",
#     time=0.0
# )

# mike = Agent(
#     name="mike",
#     age=35,
#     chat=mike_chat,
#     memory=mike_memory,
#     retriever=mike_retriever,
#     behavior=mike_behavior,
#     planner=mike_planner,
#     vad=vad_agent,
# )

agent_dict = {
    maru.name: maru,
    # ethan.name: ethan,
    # anya.name: anya,
    # mike.name: mike,
}