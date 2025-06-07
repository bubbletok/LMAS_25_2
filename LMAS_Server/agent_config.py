from agent import Agent
from agent_chat import AgentChat
from agent_memory import AgentMemory
from agent_behavior import AgentBehavior
from agent_planner import AgentPlanner
from agent_vad import AgentVAD

from langchain_community.chat_models import ChatOllama

agent_llm_model_name = "phi4-mini"
vad_agent_model_name = "phi4-mini"
# agent_chat_llm = ChatOllama(model = agent_llm_model_name)
vad_agent_chat_llm = ChatOllama(model = vad_agent_model_name)

vad_chat = AgentChat(llm=vad_agent_chat_llm)
vad_agent = AgentVAD(chat=vad_chat)

maru_chat_llm = ChatOllama(model=agent_llm_model_name)
maru_chat = AgentChat(llm=maru_chat_llm)
maru_memory = AgentMemory(chat=maru_chat)
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
    behavior=maru_behavior,
    planner=maru_planner,
    vad=vad_agent,
)


ethan_chat_llm = ChatOllama(model=agent_llm_model_name)
ethan_chat = AgentChat(llm=ethan_chat_llm)
ethan_memory = AgentMemory(chat=ethan_chat)
ethan_behavior = AgentBehavior(chat=ethan_chat)
ethan_planner = AgentPlanner(chat=ethan_chat)

ethan_memory.add_memory(
    memory_content="My name is Ethan, I am 25 years old",
    emotion="Neutral",
    time=0.0
)

ethan = Agent(
    name="ethan",
    age=25,
    chat=ethan_chat,
    memory=ethan_memory,
    behavior=ethan_behavior,
    planner=ethan_planner,
    vad=vad_agent,
)

agent_dict = {
    "maru": maru,
    "ethan": ethan
}

agent_list = [maru, ethan]