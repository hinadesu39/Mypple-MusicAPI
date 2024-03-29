﻿using CommonHelper;
using FluentValidation;
using MusicInfrastructure;

namespace MusicAdmin.WebAPI.Musics.Request
{
    public record MusicAddRequest(
        Uri AudioUrl,
        Uri? MusicPicUrl,
        string Title,
        double Duration,
        string Artist,
        string Album,
        string? Type,
        string? Lyric,
        int PublishTime      
    );

    public class MusicAddRequestValidator : AbstractValidator<MusicAddRequest>
    {
        public MusicAddRequestValidator(MusicDBContext ctx)
        {
            RuleFor(x => x.AudioUrl).NotEmpty();
            RuleFor(x => x.Title).NotEmpty().Length(1, 200);
            RuleFor(x => x.Duration != 0);
            RuleFor(x => x.Artist).NotEmpty().Length(1, 200);
            RuleFor(x => x.Album).NotEmpty().Length(1, 200);
            RuleFor(x => x.PublishTime != 0);
        }
    }
}
