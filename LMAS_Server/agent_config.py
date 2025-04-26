from agent import Agent
from agent_chat import AgentChat
from agent_memory import AgentMemory
from agent_behavior import AgentBehavior
from agent_planner import AgentPlanner
from agent_vad import AgentVAD

from langchain_community.chat_models import ChatOllama

agent_llm_model_name = "tinyllama"
vad_agent_model_name = "tinyllama"
agent_chat_llm = ChatOllama(model = agent_llm_model_name)
vad_agent_chat_llm = ChatOllama(model = vad_agent_model_name)

maru_chat = AgentChat(llm=agent_chat_llm)
maru_memory = AgentMemory()
maru_behavior = AgentBehavior()
maru_planner = AgentPlanner()

ethan_chat = AgentChat(llm=agent_chat_llm)
ethan_memory = AgentMemory()
ethan_behavior = AgentBehavior()
ethan_planner = AgentPlanner()

vad_chat = AgentChat(llm=vad_agent_chat_llm)
vad_agent = AgentVAD(chat=vad_chat)

maru = Agent(
    name="Maru",
    age=5,
    chat=maru_chat,
    memory=maru_memory,
    behavior=maru_behavior,
    planner=maru_planner,
    vad=vad_agent,
)

ethan = Agent(
    name="Ethan",
    age=25,
    chat=ethan_chat,
    memory=ethan_memory,
    behavior=ethan_behavior,
    planner=ethan_planner,
    vad=vad_agent,
)

agent_list = [maru, ethan]