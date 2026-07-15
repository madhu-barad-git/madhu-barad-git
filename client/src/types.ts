export interface PollOption {
  id: string;
  text: string;
  voteCount: number;
}

export interface Poll {
  id: string;
  question: string;
  createdAt: string;
  isClosed: boolean;
  options: PollOption[];
}

export interface CreatePollRequest {
  question: string;
  options: string[];
}

export interface VoteRequest {
  optionId: string;
  voterKey: string;
}

export interface ApiError {
  message: string;
}
