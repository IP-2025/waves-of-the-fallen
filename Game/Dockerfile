# 1) Grab Godot + install .NET into /opt/dotnet
FROM barichello/godot-ci:mono-4.3 AS dotnet
WORKDIR /opt/dotnet
RUN wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh \
 && chmod +x dotnet-install.sh \
 # install both SDK *and* the runtime (hostfxr) for .NET 6 LTS
 && ./dotnet-install.sh --channel 8.0 --install-dir /opt/dotnet \
 && ./dotnet-install.sh --channel 8.0 --runtime dotnet --install-dir /opt/dotnet

# 2) Copy in your project, generate glue
FROM barichello/godot-ci:mono-4.3 AS builder
COPY --from=dotnet /opt/dotnet /opt/dotnet
ENV DOTNET_ROOT=/opt/dotnet
ENV PATH=$PATH:/opt/dotnet
WORKDIR /app
COPY . /app
RUN godot --headless --generate-mono-glue /app

# 3) Final image
FROM barichello/godot-ci:mono-4.3
# bring in both your code & the dotnet install
COPY --from=builder /app /app
COPY --from=builder /opt/dotnet /opt/dotnet

ENV DOTNET_ROOT=/opt/dotnet
ENV PATH=$PATH:/opt/dotnet

EXPOSE 3000/udp
EXPOSE 9999/udp
ENTRYPOINT ["godot","--headless", "--server-mode","--path","/app"]
