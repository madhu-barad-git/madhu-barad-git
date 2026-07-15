import { apiClient } from './client';
import type { CreatePollRequest, Poll, VoteRequest } from '../types';

export async function createPoll(request: CreatePollRequest): Promise<Poll> {
  const { data } = await apiClient.post<Poll>('/polls', request);
  return data;
}

export async function getPoll(pollId: string): Promise<Poll> {
  const { data } = await apiClient.get<Poll>(`/polls/${pollId}`);
  return data;
}

export async function vote(pollId: string, request: VoteRequest): Promise<Poll> {
  const { data } = await apiClient.post<Poll>(`/polls/${pollId}/vote`, request);
  return data;
}

export async function closePoll(pollId: string): Promise<Poll> {
  const { data } = await apiClient.post<Poll>(`/polls/${pollId}/close`);
  return data;
}
