# from pydantic import BaseModel, Field
from langchain_core.pydantic_v1 import BaseModel, Field
from langchain_core.prompts import PromptTemplate
from langchain_core.documents import Document
from typing import Optional, List, Dict, Tuple
import re

from agent_chat import AgentChat
# from agent_retreiver import AgentRetriever

class AgentMemory(BaseModel):
    """Agent memory for storing and retrieving information."""
    # name: str = Field(description="Name of the agent")
    # retriever: AgentRetriever = Field(description="Retriever for the agent memory")
    working_memory: List[Document] = Field(
        default_factory=list, description="Working memory of the agent"
    )
    
    mid_term_memory: List[Document] = Field(
        default_factory=list, description="Mid-term memory of the agent"
    )
    
    long_term_memory: List[Document] = Field(
        default_factory=list, description="Long-term memory of the agent"
    )
    chat: AgentChat = Field(description="Chat model for the agent memory")
    
    consolidate_threshold: float = Field(
        default=0.5, description="Threshold for consolidating memory"
    )
    importance_score_weight: float = Field(
        default=0.15, description="Weight for importance score in memory consolidation"
    )
    
    # for pydantic_v1
    class Config:
        arbitrary_types_allowed = True
    # for pydantic_v2
    # model_config = {"arbitrary_types_allowed": True}
    
    def __str__(self):
        return self.dict()
    
    def update_memory(self, observation: str, emotion: str, time: float):
        """Update the agent's memory with a new observation."""
        return self.add_memory(observation, emotion, time)
        # self.consolidate_memory(time)
        
    def add_memory(self, memory_content: str, emotion: str, time: float):
        """Add memory to the agent's working memory."""
        importance = self.score_memory_importance(memory_content, emotion)
        doc = Document(page_content=memory_content, metadata={"importance": importance, "time": time})
        self.working_memory.append(doc)
        
        return doc
        
    def score_memory_importance(self, memory_content: str, emotion: str) -> float:
        """Score the importance of a memory based on its content."""
        prompt = f'''Your task is to evaluate the importance of a memory based on its content and associated emotion.
        Think step by step:
        1. Consider the type of emotion: for example, "joy" may indicate a celebratory moment, while "regret" may reflect a learning experience.
        2. Examine the memory content for personal significance, consequence, or uniqueness.
        3. Estimate the importance of the memory on a scale from 0.0 (not important at all) to 1.0 (extremely important).

        Output only a single float number between 0.0 and 1.0, with no explanation or extra text.

        Emotion: {emotion}  
        Memory: {memory_content}  
        Importance:'''
        # Parse the result to extract emotion
        response = self.chat.llm.invoke(prompt)
        result = response.content

        match = re.search(r"[-+]?\d*\.\d+|\d+", result)
        if match:
            return float(match.group(0)) # * self.importance_score_weight
        else:
            return 0.0
    
    def get_summary(self) -> str:
        """Get the summary of the agent's memory."""
        # Placeholder for summary logic
        # In a real implementation, this could involve summarizing the working memory
        # For now, we will just return the working memory as a string
        all_memory = "\n".join([doc.page_content for doc in self.working_memory])
        all_memory = all_memory if all_memory.strip() else '[EMPTY]'
        print(f'All memory: {all_memory}')
        prompt = f"""
        You are a helpful assistant that summarizes memory logs.

        Your task is to return a **single, concise summary string** of the following text.

        Instructions:
        - DO NOT explain anything.
        - DO NOT include any extra formatting or labels.
        - If there are no memories, respond exactly: No memories available.

        Memories:
        \"\"\"
        {all_memory}
        \"\"\"
        """
        response = self.chat.llm.invoke(prompt)
        result = response.content
        return result
    
    def consolidate_memory(self, time: float):
        """Consolidate memory based on importance and time."""
        # Placeholder for consolidation logic
        # In a real implementation, this could involve moving memories to mid-term or long-term storage
        # For now, we will just print the working memory
        for working_doc in self.working_memory:
            working_doc_importance = working_doc.metadata["importance"]
            working_doc_time = working_doc.metadata["time"]
            # Apply the forgetting curve
            
            if working_doc.metadata["time"] < time:
                # Move to mid-term memory
                self.mid_term_memory.append(working_doc)
                self.working_memory.remove(working_doc)
        for doc in self.working_memory:
            print(f"Memory: {doc.page_content}, Importance: {doc.metadata['importance']}, Time: {doc.metadata['time']}")
        
    def fetch_memory(self, content: str) -> List[Document]:
        """Fetch relevant memory based on the query."""
        # Placeholder for retrieval logic
        # In a real implementation, this could involve searching through the memory
        # For now, we return all working memory
        return self.working_memory
    