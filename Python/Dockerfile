FROM python:3.8-slim

COPY requirements/production.txt /requirements.txt
RUN pip install -r /requirements.txt

COPY ./ /medovuha
RUN pip install -e /medovuha

EXPOSE 9999
CMD ["python", "-m", "medovuha.server"]
