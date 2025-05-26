from langchain_core.retrievers import BaseRetriever
from langchain_core.documents import Document
from langchain_core.pydantic_v1 import Field
from langchain_core.vectorstores import VectorStore
from typing import Any, Dict, List, Optional, Tuple
from copy import deepcopy

class AgentRetriever(BaseRetriever):
    """Agent retriever for retrieving information from the agent's memory."""
    vectorstore : VectorStore
    alpha_recency: float = Field(default=1) 
    alpha_importance : float = Field(default=0.01)
    alpha_vad: float = Field(default=0.1)

    search_kwargs: dict = Field(default_factory=lambda: dict(k=100,)) # 유사도 검색 수행 시 가져올 Memory의 수(Default : k=100)
    index_stage: List[Document] = Field(default_factory=list)
    default_salience: Optional[float] = None
    k: int = 15
    
    class Config:
        arbitrary_types_allowed = True
        
    def _get_combined_score(self, document: Document, current_time: float) -> float:
        times_passed = current_time - document.metadata.get("last_accessed_at") #self._get_times_passed(current_time, self._document_get_date("last_accessed_at", document))
        search_count = document.metadata.get("search_count", 0)
        importance = document.metadata.get("importance", 1)
        emotion = document.metadata.get("emotion", 0)
        score = self.alpha_importance * importance * search_count + (1 + self.alpha_recency * times_passed) + self.alpha_vad * emotion
        return score
    
    def get_salient_docs(self, query: str) -> Dict[int, Tuple[Document, float]]:
        docs_and_scores: List[Tuple[Document, float]]
        docs_and_scores = self.vectorstore.similarity_search_with_relevance_scores(query,namespace = self.name ,**self.search_kwargs)
        results = {}
        for fetched_doc, relevance in docs_and_scores:
            if "buffer_idx" in fetched_doc.metadata:
                buffer_idx = int(fetched_doc.metadata["buffer_idx"])
                doc = self.index_stage[buffer_idx]
                results[buffer_idx] = (doc, relevance)
        return results
    
    def _get_rescored_docs(self, now : float, docs_and_scores: Dict[Any, Tuple[Document, Optional[float]]]) -> List[Document]:
        current_time = now
        rescored_docs = [(doc, self._get_combined_score(doc, relevance, current_time)) for doc, relevance in docs_and_scores.values()]
        rescored_docs.sort(key=lambda x: x[1], reverse=True)
        result = []
        for doc, _ in rescored_docs[: self.k]:
            buffered_doc = self.index_stage[doc.metadata["buffer_idx"]]
            buffered_doc.metadata["last_accessed_at"] = current_time
            if "search_count" in buffered_doc.metadata:
                buffered_doc.metadata["search_count"] += 1
            else:
                buffered_doc.metadata["search_count"] = 1
            result.append(buffered_doc)
        return result
    
    def _get_relevant_documents(self, query: str, now: float) -> List[Document]:
        docs_and_scores = {doc.metadata["buffer_idx"]: (doc, self.default_salience) for doc in self.index_stage[-self.k :]}
        docs_and_scores.update(self.get_salient_docs(query))
        return self._get_rescored_docs(now, docs_and_scores)
    
    def add_documents(self, documents: List[Document], time: float, **kwargs: Any) -> List[str]:
        dup_docs = [deepcopy(d) for d in documents]
        for i, doc in enumerate(dup_docs):
            if "last_accessed_at" not in doc.metadata:
                doc.metadata["last_accessed_at"] = time
            if "created_at" not in doc.metadata:
                doc.metadata["created_at"] = time
            if "search_count" not in doc.metadata:
                doc.metadata["search_count"] = 0
            doc.metadata["buffer_idx"] = len(self.index_stage) + i
        self.index_stage.extend(dup_docs)
        return self.vectorstore.add_documents(dup_docs, **kwargs)