from fastapi import FastAPI, HTTPException
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
                "agent": maru.__str__(),
            },
            {
                "agent": ethan.__str__(),
            }
        ]
    }

@app.get("/agent/{agent_name}")
async def get_agent_info(agent_name: str):
    """Get information about a specific agent"""
    try:
        agent = agent_list[0]
        return{
            "agent": agent.__str__(),
        }
    except ValueError:
        raise HTTPException(status_code=404, detail=f"Agent {agent_name} not found")

if __name__ == "__main__":
    uvicorn.run(
        "main:app", 
        host="127.0.0.1", 
        port=8000
    )
