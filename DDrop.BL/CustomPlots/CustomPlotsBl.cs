﻿using System;
using System.Threading.Tasks;
using AutoMapper;
using DDrop.BE.Models;
using DDrop.DAL;
using DDrop.Db.DbEntities;

namespace DDrop.BL.CustomPlots
{
    public class CustomPlotsBl : ICustomPlotsBl
    {
        private readonly IDDropRepository _dDropRepository;
        private readonly IMapper _mapper;

        public CustomPlotsBl(IDDropRepository dDropRepository, IMapper mapper)
        {
            _dDropRepository = dDropRepository;
            _mapper = mapper;
        }

        public async Task CreatePlot(Plot plot)
        {
            await _dDropRepository.CreatePlot(_mapper.Map<Plot, DbPlot>(plot));
        }

        public async Task DeletePlot(Plot plot)
        {
            await _dDropRepository.DeletePlot(_mapper.Map<Plot, DbPlot>(plot));
        }

        public async Task UpdatePlot(Plot plot)
        {
            var dbPlot = _mapper.Map<Plot, DbPlot>(plot);

            await _dDropRepository.UpdatePlot(dbPlot.PlotId, dbPlot.Points);
        }

        public async Task UpdatePlotName(string text, Guid plotId)
        {
            await Task.Run(() => _dDropRepository.UpdatePlotName(text, plotId));
        }
    }
}