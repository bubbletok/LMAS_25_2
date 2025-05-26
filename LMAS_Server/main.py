from fastapi import FastAPI, HTTPException
from fastapi.responses import JSONResponse
from fastapi.encoders import jsonable_encoder
from fastapi.middleware.cors import CORSMiddleware
import uvicorn

from agent_config import *

app = FastAPI()


app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

@app.get("/")
async def read_root():
    return {"message": "LangChain Ollama API Server is running"}

@app.get("/agents")
async def list_agents():
    """List all available agents in the system"""
    # For now, we only have Maru
    return {
        "agents": [
            {
                "agent": maru
            },
            {
                "agent": ethan
            }
        ]
    }

@app.get("/agent/{agent_name}")
async def get_agent_info(agent_name: str):
    """Get information about a specific agent"""
    try:
        agent = get_agent(agent_name)
        agent_data = jsonable_encoder(
            agent,
            exclude_none = True,
        )
        return JSONResponse(
            content=agent_data,
        )
    except ValueError:
        raise HTTPException(status_code=404, detail=f"Agent {agent_name} not found")


def get_agent(agent_name: str):
    """Get a specific agent by name"""
    try:
        agent = agent_dict[agent_name]
        # agent = next(agent for agent in agent_list if agent.name.lower() == agent_name.lower())
        return agent
    except ValueError:
        raise HTTPException(status_code=404, detail=f"Agent {agent_name} not found")

@app.post("/agent/{agent_name}/analyze_emotion")
async def analyze_emotion(agent_name: str, prompt: str):
    """Analyze the emotion of a specific agent"""
    print(f"Analyzing emotion for agent: {agent_name} with prompt: {prompt}")
    try:
        agent = get_agent(agent_name)
        result = agent.analyze_emotion(prompt)
        return {
            "valence": result[0],
            "arousal": result[1],
            "dominance": result[2]
        }
    except ValueError:
        raise HTTPException(status_code=404, detail=f"Agent {agent_name} not found")

@app.post("/agent/{agent_name}/generate_emotion")
async def generate_emotion(agent_name: str, prompt: str):
    """Generate emotion for a specific agent"""
    try:
        agent = get_agent(agent_name)
        result = agent.generate_emotion(prompt)
        # return {
        #     "emotion": result
        # }
        return result
    except ValueError:
        raise HTTPException(status_code=404, detail=f"Agent {agent_name} not found")

@app.post("/agent/{agent_name}/observe")
async def observe(agent_name: str, observation: str, time: float):
    """Observe the environment and update memory for a specific agent"""
    try:
        agent = get_agent(agent_name)
        result = agent.observe(observation, time)
        return result
    except ValueError:
        raise HTTPException(status_code=404, detail=f"Agent {agent_name} not found")

@app.post("/agent/{agent_name}/summary")
async def get_summary(agent_name: str):
    """Get the summary of a specific agent's memory"""
    try:
        agent = get_agent(agent_name)
        result = agent.get_summary()
        return result
    except ValueError:
        raise HTTPException(status_code=404, detail=f"Agent {agent_name} not found")


if __name__ == "__main__":
    uvicorn.run(    
        "main:app", 
        host="127.0.0.1", 
        port=8000
    )
